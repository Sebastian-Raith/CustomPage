
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;


namespace BackendRaith.Services
{
    public class WebScrapingService
    {
        private readonly string _username;
        private readonly string _password;

        public WebScrapingService()
        {
            _username = Environment.GetEnvironmentVariable("PREP_Username");
            _password = Environment.GetEnvironmentVariable("PREP_Password");
        }

        public List<Shift> ScrapeData()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            using var driver = new ChromeDriver(chromeOptions);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {

                driver.Navigate().GoToUrl("https://dienstplan.o.roteskreuz.at");


                var usernameField = wait.Until(d => d.FindElement(By.Name("login")));
                var passwordField = driver.FindElement(By.Name("password"));
                var loginButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Login']"));

                usernameField.SendKeys(_username);
                passwordField.SendKeys(_password);
                loginButton.Click();
                Thread.Sleep(3000);


                var meineDiensteLink = wait.Until(d => d.FindElement(By.XPath("//a[@href='/StaffPortal/duties.php']")));
                meineDiensteLink.Click();
                Thread.Sleep(3000);

                wait.Until(d => d.Url.Contains("/StaffPortal/duties.php"));


                var tableBody = wait.Until(d => d.FindElement(By.Id("inDataTableBody_1")));
                var rows = tableBody.FindElements(By.TagName("tr"));

                var shifts = new List<Shift>();

                foreach (var row in rows)
                {
                    var cells = row.FindElements(By.TagName("td"));
                    var shift = new Shift
                    {
                        Date = ParseDate(cells[0].Text.Trim()),
                        ShiftName = cells[1].Text.Trim(),
                        StartTime = ParseTime(cells[2].Text.Trim()),
                        EndTime = ParseTime(cells[3].Text.Trim()),
                        Duration = ParseTimeOnly(cells[4].Text.Trim()),
                        Activity = cells[5].Text.Trim(),
                        Department = cells[6].Text.Trim(),
                        AllocationInfo = cells[7].Text.Trim(),
                        DutyType = cells[8].Text.Trim(),
                        Info = cells[9].Text.Trim()
                    };

                    shifts.Add(shift);
                }
                return shifts;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error during web scraping: {ex.Message}");
                throw;
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        private string ParseDate(string date)
        {
            if (DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result.ToString("yyyy-MM-dd");
            }
            throw new FormatException($"Invalid date format: {date}");
        }
        private string ParseTime(string time)
        {
            if (DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result.ToString("HH:mm");
            }
            throw new FormatException($"Invalid time format: {time}");
        }
        private string ParseTimeOnly(string duration)
        {
            var formattedDuration = duration.Replace(',', ':').Replace('.', ':');
            if (TimeOnly.TryParseExact(formattedDuration, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result.ToString("HH:mm");
            }
            throw new FormatException($"Invalid duration format: {duration}");
        }

    }
}
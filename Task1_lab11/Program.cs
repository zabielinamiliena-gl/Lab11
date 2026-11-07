using System;

namespace BukovynaAirport
{
    class Journal
    {
        private int minutes;

        public Journal(int startHour, int startMinute)
        {
            minutes = startHour * 60 + startMinute;
        }

        public void Write(string source, string message)
        {
            int hour = minutes / 60;
            int minute = minutes % 60;
            Console.WriteLine("[" + hour.ToString("00") + ":" + minute.ToString("00") + "] [" + source + "] " + message);
            minutes += 1;
        }
    }

    class WeatherData
    {
        public double TemperatureC { get; }
        public double WindMs { get; }
        public int VisibilityM { get; }

        public WeatherData(double temperatureC, double windMs, int visibilityM)
        {
            TemperatureC = temperatureC;
            WindMs = windMs;
            VisibilityM = visibilityM;
        }

        public string Text()
        {
            return TemperatureC.ToString("0.0") + " °C, вітер " + WindMs.ToString("0.0") + " м/с, видимість " + VisibilityM + " м";
        }
    }

    interface IWeatherSource
    {
        WeatherData Read();
    }

    class ForeignWeatherService
    {
        public string Name { get { return "SkyData"; } }

        public double TemperatureFahrenheit { get { return 68; } }
        public double WindKnots { get { return 10; } }
        public double VisibilityMiles { get { return 5; } }

        public void ReportRaw(Journal journal)
        {
            journal.Write(Name, "Вихідні дані: температура " + TemperatureFahrenheit + " °F, вітер " + WindKnots + " вузлів, видимість " + VisibilityMiles + " миль");
        }
    }

    class LocalMetarService
    {
        public string Name { get { return "Буковина-Мет"; } }

        public double TemperatureC { get { return 3; } }
        public double WindKmh { get { return 50; } }
        public double VisibilityKm { get { return 0.6; } }

        public void ReportRaw(Journal journal)
        {
            journal.Write(Name, "Вихідні дані: температура " + TemperatureC + " °C, вітер " + WindKmh + " км/год, видимість " + VisibilityKm + " км");
        }
    }

    class ForeignWeatherAdapter : IWeatherSource
    {
        private readonly ForeignWeatherService service;
        private readonly Journal journal;

        public ForeignWeatherAdapter(ForeignWeatherService service, Journal journal)
        {
            this.service = service;
            this.journal = journal;
        }

        public WeatherData Read()
        {
            service.ReportRaw(journal);
            double celsius = (service.TemperatureFahrenheit - 32) * 5.0 / 9.0;
            double wind = service.WindKnots * 0.514;
            int visibility = (int)Math.Round(service.VisibilityMiles * 1609);
            WeatherData data = new WeatherData(Math.Round(celsius, 1), Math.Round(wind, 1), visibility);
            journal.Write("Адаптер " + service.Name, "Перетворено для центру: " + data.Text());
            return data;
        }
    }

    class LocalMetarAdapter : IWeatherSource
    {
        private readonly LocalMetarService service;
        private readonly Journal journal;

        public LocalMetarAdapter(LocalMetarService service, Journal journal)
        {
            this.service = service;
            this.journal = journal;
        }

        public WeatherData Read()
        {
            service.ReportRaw(journal);
            double wind = service.WindKmh / 3.6;
            int visibility = (int)Math.Round(service.VisibilityKm * 1000);
            WeatherData data = new WeatherData(Math.Round(service.TemperatureC, 1), Math.Round(wind, 1), visibility);
            journal.Write("Адаптер " + service.Name, "Перетворено для центру: " + data.Text());
            return data;
        }
    }

    class FlightControlCenter
    {
        private readonly Journal journal;

        public FlightControlCenter(Journal journal)
        {
            this.journal = journal;
        }

        public void DecideTakeoff(IWeatherSource source)
        {
            WeatherData data = source.Read();
            journal.Write("Центр управління", "Отримано через власний інтерфейс: " + data.Text());
            if (data.WindMs <= 12 && data.VisibilityM >= 1000)
                journal.Write("Центр управління", "Зліт дозволено");
            else
                journal.Write("Центр управління", "Зліт заборонено");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Situation1();
        }

        static void Situation1()
        {
            Journal journal = new Journal(8, 0);
            FlightControlCenter center = new FlightControlCenter(journal);

            ForeignWeatherService foreign = new ForeignWeatherService();
            center.DecideTakeoff(new ForeignWeatherAdapter(foreign, journal));

            LocalMetarService local = new LocalMetarService();
            center.DecideTakeoff(new LocalMetarAdapter(local, journal));
        }
    }
}

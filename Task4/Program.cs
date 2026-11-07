using System;
using System.Collections.Generic;

namespace FlightLogSystem
{
    public enum UserRole
    {
        Dispatcher,  
        FlightSafety,  
        Cashier     
    }

    public class User
    {
        public string Name { get; }
        public UserRole Role { get; }

        public User(string name, UserRole role)
        {
            Name = name;
            Role = role;
        }
    }

    public interface IFlightLog
    {
        string FlightNumber { get; }
        void DisplaySummary();
        string GetTelemetry(User user);
    }

    public class RealFlightLog : IFlightLog
    {
        public string FlightNumber { get; }
        private string _telemetryData;

        public RealFlightLog(string flightNumber)
        {
            FlightNumber = flightNumber;
            LoadTelemetryFromArchive();
        }

        private void LoadTelemetryFromArchive()
        {
            Console.WriteLine($"[АРХІВ] Триває дороге завантаження телеметрії для рейсу {FlightNumber}...");
            _telemetryData = $"Телеметрія рейсу {FlightNumber}: [Висота: 10000м, Швидкість: 850 км/год, Стан систем: OK]";
        }

        public void DisplaySummary()
        {
            Console.WriteLine($"Рейс: {FlightNumber}");
        }

        public string GetTelemetry(User user)
        {
            return _telemetryData;
        }
    }

    public class FlightLogProxy : IFlightLog
    {
        public string FlightNumber { get; }
        private RealFlightLog _realFlightLog;

        public FlightLogProxy(string flightNumber)
        {
            FlightNumber = flightNumber;
        }

        public void DisplaySummary()
        {
            Console.WriteLine($"Рейс: {FlightNumber}");
        }

        public string GetTelemetry(User user)
        {
            if (user.Role != UserRole.Dispatcher && user.Role != UserRole.FlightSafety)
            {
                return "Недостатньо прав для перегляду бортового журналу.";
            }

            if (_realFlightLog == null)
            {
                _realFlightLog = new RealFlightLog(FlightNumber);
            }

            return _realFlightLog.GetTelemetry(user);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<IFlightLog> flightLogs = new List<IFlightLog>
            {
                new FlightLogProxy("PS-101"),
                new FlightLogProxy("PS-102"),
                new FlightLogProxy("PS-103"),
                new FlightLogProxy("PS-104"),
                new FlightLogProxy("PS-105")
            };

            var dispatcher = new User("Олексій", UserRole.Dispatcher);
            var cashier = new User("Марія", UserRole.Cashier);

            Console.WriteLine("=== 1. Відображення списку з 5 рейсів ===");
            foreach (var log in flightLogs)
            {
                log.DisplaySummary();
            }
            Console.WriteLine();

            Console.WriteLine("=== 2. Диспетчер запитує телеметрію PS-101 (Перше звернення) ===");
            var ps101 = flightLogs[0];
            Console.WriteLine(ps101.GetTelemetry(dispatcher));
            Console.WriteLine();

            Console.WriteLine("=== 2b. Диспетчер запитує телеметрію PS-101 (Повторне звернення) ===");
            Console.WriteLine(ps101.GetTelemetry(dispatcher));
            Console.WriteLine();

            Console.WriteLine("=== 3. Касир намагається відкрити телеметрію PS-102 ===");
            var ps102 = flightLogs[1];
            Console.WriteLine(ps102.GetTelemetry(cashier));
        }
    }
}
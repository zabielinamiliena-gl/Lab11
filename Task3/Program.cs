using System;

namespace AirportCheckIn
{

    public class DocumentVerificationService
    {
        public bool VerifyDocuments(string passengerName, string passportNumber)
        {
            Console.WriteLine($"[1/5 Перевірка документів] Перевірка паспорта №{passportNumber} пасажира {passengerName}...");
            return true;
        }
    }

    public class SeatAssignmentService
    {
        public string AssignSeat(string passengerName)
        {
            string seat = "14A";
            Console.WriteLine($"[2/5 Розміщення в салоні] Пасажиру {passengerName} призначено місце {seat}.");
            return seat;
        }
    }

    public class BaggageService
    {
        public void ProcessBaggage(string passengerName, int baggageCount)
        {
            Console.WriteLine($"[3/5 Прийом багажу] Оформлено багаж ({baggageCount} шт.) для пасажира {passengerName}.");
        }
    }

    public class BoardingPassPrinter
    {
        public void PrintBoardingPass(string passengerName, string seat)
        {
            Console.WriteLine($"[4/5 Друк талона] Друкується посадковий талон: Пасажир: {passengerName}, Місце: {seat}.");
        }
    }

    public class BorderControlService
    {
        public void NotifyBorderControl(string passengerName, string passportNumber)
        {
            Console.WriteLine($"[5/5 Прикордонна служба] Надіслано сповіщення про пасажира {passengerName} ({passportNumber}).");
        }
    }


    public class CheckInFacade
    {
        private readonly DocumentVerificationService _documentVerification;
        private readonly SeatAssignmentService _seatAssignment;
        private readonly BaggageService _baggage;
        private readonly BoardingPassPrinter _boardingPassPrinter;
        private readonly BorderControlService _borderControl;

        public CheckInFacade(
            DocumentVerificationService documentVerification,
            SeatAssignmentService seatAssignment,
            BaggageService baggage,
            BoardingPassPrinter boardingPassPrinter,
            BorderControlService borderControl)
        {
            _documentVerification = documentVerification;
            _seatAssignment = seatAssignment;
            _baggage = baggage;
            _boardingPassPrinter = boardingPassPrinter;
            _borderControl = borderControl;
        }

        public void RegisterPassenger(string passengerName, string passportNumber, int baggageCount)
        {
            Console.WriteLine($"=== Розпочато процес реєстрації: {passengerName} ===");
            
            _documentVerification.VerifyDocuments(passengerName, passportNumber);
            string seat = _seatAssignment.AssignSeat(passengerName);
            _baggage.ProcessBaggage(passengerName, baggageCount);
            _boardingPassPrinter.PrintBoardingPass(passengerName, seat);
            _borderControl.NotifyBorderControl(passengerName, passportNumber);

            Console.WriteLine($"=== Реєстрацію пасажира {passengerName} успішно завершено ===\n");
        }
    }


    public class CheckInDesk
    {
        private readonly CheckInFacade _facade;

        public CheckInDesk(CheckInFacade facade)
        {
            _facade = facade;
        }

        public void ProcessCheckIn(string passengerName, string passportNumber, int baggageCount)
        {
            Console.WriteLine("[Стійка реєстрації]");
            _facade.RegisterPassenger(passengerName, passportNumber, baggageCount);
        }
    }

    public class SelfServiceKiosk
    {
        private readonly CheckInFacade _facade;

        public SelfServiceKiosk(CheckInFacade facade)
        {
            _facade = facade;
        }

        public void ProcessCheckIn(string passengerName, string passportNumber, int baggageCount)
        {
            Console.WriteLine("[Кіоск самореєстрації]");
            _facade.RegisterPassenger(passengerName, passportNumber, baggageCount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ініціалізація підсистем
            var docVerification = new DocumentVerificationService();
            var seatAssignment = new SeatAssignmentService();
            var baggage = new BaggageService();
            var printer = new BoardingPassPrinter();
            var borderControl = new BorderControlService();

            // Ініціалізація фасаду
            var checkInFacade = new CheckInFacade(
                docVerification, 
                seatAssignment, 
                baggage, 
                printer, 
                borderControl
            );

            var desk = new CheckInDesk(checkInFacade);
            var kiosk = new SelfServiceKiosk(checkInFacade);

            desk.ProcessCheckIn("Олексій Ковальов", "AB123456", 2);

            kiosk.ProcessCheckIn("Марія Сидоренко", "CD789012", 1);

            Console.WriteLine("[Окремий виклик підсистеми друку]");
            printer.PrintBoardingPass("Олексій Ковальов", "14A");
        }
    }
}
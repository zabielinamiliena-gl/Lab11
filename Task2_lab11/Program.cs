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

    interface ITicket
    {
        string Description();
        int Price();
    }

    class BaseTicket : ITicket
    {
        private readonly string passenger;
        private readonly string flightNumber;
        private readonly Journal journal;
        private readonly int price;

        public BaseTicket(string passenger, string flightNumber, Journal journal)
        {
            this.passenger = passenger;
            this.flightNumber = flightNumber;
            this.journal = journal;
            price = 2500;
        }

        public string Description()
        {
            return "Квиток " + passenger + ", рейс " + flightNumber;
        }

        public int Price()
        {
            journal.Write("Базовий квиток", "Вартість " + price + " грн");
            return price;
        }
    }

    abstract class TicketExtra : ITicket
    {
        protected readonly ITicket inner;
        protected readonly Journal journal;
        private readonly string title;
        private readonly int extra;

        protected TicketExtra(ITicket inner, Journal journal, string title, int extra)
        {
            this.inner = inner;
            this.journal = journal;
            this.title = title;
            this.extra = extra;
        }

        public string Description()
        {
            return inner.Description() + ", " + title.ToLower();
        }

        public int Price()
        {
            int total = inner.Price() + extra;
            journal.Write(title, "Додає " + extra + " грн, разом " + total + " грн");
            return total;
        }
    }

    class ExtraBaggage : TicketExtra
    {
        public ExtraBaggage(ITicket inner, Journal journal) : base(inner, journal, "Додатковий багаж", 800) { }
    }

    class Insurance : TicketExtra
    {
        public Insurance(ITicket inner, Journal journal) : base(inner, journal, "Страхування", 450) { }
    }

    class PriorityBoarding : TicketExtra
    {
        public PriorityBoarding(ITicket inner, Journal journal) : base(inner, journal, "Пріоритетна посадка", 350) { }
    }

    class Catering : TicketExtra
    {
        public Catering(ITicket inner, Journal journal) : base(inner, journal, "Харчування", 200) { }
    }

    class SeatChoice : TicketExtra
    {
        public SeatChoice(ITicket inner, Journal journal) : base(inner, journal, "Вибір місця", 250) { }
    }

    class TicketDesk
    {
        private readonly Journal journal;

        public TicketDesk(Journal journal)
        {
            this.journal = journal;
        }

        public void Sell(ITicket ticket)
        {
            int price = ticket.Price();
            journal.Write("Каса", ticket.Description() + ". До сплати " + price + " грн");
            Console.WriteLine(ticket.Description());
            Console.WriteLine("Вартість: " + price + " грн");
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Situation2();
        }

        static void Situation2()
        {
            Journal journal = new Journal(9, 0);
            TicketDesk desk = new TicketDesk(journal);

            ITicket first = new BaseTicket("Олена Коваль", "PS-101", journal);
            desk.Sell(first);

            first = new ExtraBaggage(first, journal);
            first = new Insurance(first, journal);
            first = new PriorityBoarding(first, journal);
            desk.Sell(first);

            ITicket second = new BaseTicket("Марина Іваненко", "PS-102", journal);
            second = new Catering(second, journal);
            second = new Catering(second, journal);
            second = new SeatChoice(second, journal);
            desk.Sell(second);
        }
    }
}

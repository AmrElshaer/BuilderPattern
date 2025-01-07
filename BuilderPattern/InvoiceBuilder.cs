namespace BuilderPattern;
public record PoundsShillingsPence
{
    public int Pounds { get; }
    public int Shillings { get; }
    public int Pence { get; }

    public static readonly PoundsShillingsPence Zero = new(0, 0, 0);

    public PoundsShillingsPence(int pounds, int shillings, int pence)
    {
        // Normalize values
        if (pence >= 12)
        {
            shillings += pence / 12;
            pence %= 12;
        }
        if (shillings >= 20)
        {
            pounds += shillings / 20;
            shillings %= 20;
        }

        if (shillings is < 0 or > 19)
            throw new ArgumentException("Shillings must be between 0 and 19");
        if (pence is < 0 or > 11)
            throw new ArgumentException("Pence must be between 0 and 11");

        Pounds = pounds;
        Shillings = shillings;
        Pence = pence;
    }

    // Method to calculate a percentage of this amount
    public PoundsShillingsPence CalculatePercentage(double percentage)
    {
        // Convert to total pence for calculation
        var totalPence = (Pounds * 240) + (Shillings * 12) + Pence;
        var discountPence = (int)Math.Round(totalPence * percentage);
        
        // Convert back to pounds/shillings/pence
        var newPounds = discountPence / 240;
        var remainder = discountPence % 240;
        var newShillings = remainder / 12;
        var newPence = remainder % 12;

        return new PoundsShillingsPence(newPounds, newShillings, newPence);
    }
}

public class InvoiceBuilder
{
    private readonly List<InvoiceLine> _lines = new();
    private Recipient _recipient = new RecipientBuilder().Build();
    private PoundsShillingsPence _discount = PoundsShillingsPence.Zero;

    public InvoiceBuilder WithLine(string description, PoundsShillingsPence unitPrice, int quantity = 1)
    {
        _lines.Add(new InvoiceLine(description, quantity, unitPrice));
        return this;
    }

    public InvoiceBuilder WithRecipient(Recipient recipient)
    {
        _recipient = recipient;
        return this;
    }

    public InvoiceBuilder WithDiscount(double percentageDiscount)
    {
        if (percentageDiscount is < 0 or > 1)
            throw new ArgumentException("Discount must be between 0 and 1");

        // Calculate total before discount
        var total = CalculateTotal();
        _discount = total.CalculatePercentage(percentageDiscount);
        return this;
    }

    public InvoiceBuilder WithDiscount(PoundsShillingsPence fixedDiscount)
    {
        _discount = fixedDiscount;
        return this;
    }

    private PoundsShillingsPence CalculateTotal()
    {
        int totalPence = 0;
        foreach (var line in _lines)
        {
            var linePence = (line.UnitPrice.Pounds * 240) + 
                           (line.UnitPrice.Shillings * 12) + 
                           line.UnitPrice.Pence;
            totalPence += linePence * line.Quantity;
        }

        var pounds = totalPence / 240;
        var remainder = totalPence % 240;
        var shillings = remainder / 12;
        var pence = remainder % 12;

        return new PoundsShillingsPence(pounds, shillings, pence);
    }
    public InvoiceBuilder But()
    {
        return new InvoiceBuilder()
            .WithRecipient(_recipient)
            .WithInvoiceLines(_lines)
            .WithDiscount(_discount);
    }

    private InvoiceBuilder WithInvoiceLines(List<InvoiceLine> lines)
    {
       _lines.AddRange(lines);
        return this;
    }

    public Invoice Build()
    {
        var invoiceLines = new InvoiceLines(_lines.ToArray());
        return new Invoice(_recipient, invoiceLines, _discount);
    }
}

public record Recipient
{
    public string Name { get; }
    public string Address { get; }

    public Recipient(string name, string address)
    {
        Name = name;
        Address = address;
    }
}

public class RecipientBuilder
{
    private string _name = "Default Name";
    private string _address = "Default Address";

    public RecipientBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public RecipientBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public Recipient Build() => new(_name, _address);
}

public record InvoiceLine
{
    public string Description { get; }
    public int Quantity { get; }
    public PoundsShillingsPence UnitPrice { get; }

    public InvoiceLine(string description, int quantity, PoundsShillingsPence unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}



public class InvoiceLines
{
    private readonly List<InvoiceLine> _lines;

    public InvoiceLines(params InvoiceLine[] lines)
    {
        _lines = new List<InvoiceLine>(lines);
    }

    public IReadOnlyList<InvoiceLine> Lines => _lines.AsReadOnly();
}

public record Invoice
{
    public Recipient Recipient { get; }
    public InvoiceLines Lines { get; }
    public PoundsShillingsPence Discount { get; }

    public Invoice(Recipient recipient, InvoiceLines lines, PoundsShillingsPence discount)
    {
        Recipient = recipient;
        Lines = lines;
        Discount = discount;
    }
}

using BuilderPattern;

Invoice invoiceWith10PercentDiscount = new InvoiceBuilder()
    .WithLine("Deerstalker Hat", new PoundsShillingsPence(0, 3, 10))
    .WithLine("Tweed Cape", new PoundsShillingsPence(0, 4, 12))
    .WithDiscount(0.10)
    .Build();

Invoice invoiceWith25PercentDiscount = new InvoiceBuilder()
    .WithLine("Deerstalker Hat", new PoundsShillingsPence(0, 3, 10))
    .WithLine("Tweed Cape", new PoundsShillingsPence(0, 4, 12))
    .WithDiscount(0.25)
    .Build();
Console.WriteLine($"Invoice with 10% discount: {invoiceWith10PercentDiscount}");
Console.WriteLine($"Invoice with 25% discount: {invoiceWith25PercentDiscount}");
// Reduce Duplication but will updte the shared products
InvoiceBuilder products = new InvoiceBuilder()
    .WithLine("Deerstalker Hat", new PoundsShillingsPence(0, 3, 10))
    .WithLine("Tweed Cape", new PoundsShillingsPence(0, 4, 12));

Invoice invoiceWith10PercentDiscount2 = products.But()
    .WithDiscount(0.10)
    .Build();

Invoice invoiceWith25PercentDiscount2 = products.But()
    .WithDiscount(0.25)
    .Build();
Console.WriteLine($"Invoice with 10% discount: {invoiceWith10PercentDiscount2}");
Console.WriteLine($"Invoice with 25% discount: {invoiceWith25PercentDiscount2}");   
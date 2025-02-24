namespace DTB.Data.App.Invoice
{
    public static class InvoiceService
    {
        public static List<InvoiceRecordDto> GetInvoiceRecordList() => new()
        {
        };

        public static List<BillDto> GetBillList() => new()
        {
            new BillDto("App Design", 24, 1, 24, "Designed UI kit & app pages."),
            new BillDto("App Customization", 26, 1, 26, "Customization & Bug Fixes."),
            new BillDto("ABC Template", 28, 1, 28, "Bootstrap 4 admin template."),
            new BillDto("App Development", 32, 1, 32, "Native App Development."),
        };

        public static List<InvoiceStateDto> GetStateList() => new()
        {
            new InvoiceStateDto("Downloaded", 1),
            new InvoiceStateDto("Draft", 2),
            new InvoiceStateDto("Paid", 3),
            new InvoiceStateDto("Partial Payment", 4),
            new InvoiceStateDto("Past Due", 5)
        };

        public static PagingData<InvoiceRecordDto> GetInvoiceRecordList(int pageIndex, int pageSize, int state)
        {
            var invoiceRecordList = GetInvoiceRecordList();

            var items = invoiceRecordList
                .Where(a => a.State == state || state == 0)
                .OrderBy(a => a.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagingData<InvoiceRecordDto>(pageIndex, pageSize, invoiceRecordList.Count, items);
        }

        public static List<string> GetpaymentMethodList() => new()
        {
            "Cash",
            "Bank Transfer",
            "Debit",
            "Credit",
            "Paypal"
        };
    }

}

internal class Program{
    public static void Main(string[] args){
        string documentPath = Environment.CurrentDirectory + "\\resources\\TabelaComputadores.csv";

        CsvReader csvReader = new CsvReader(documentPath);
        double[,] dataTable = csvReader.DataTable();

        Vogel vogel = new Vogel(dataTable);
        vogel.SetBalanceType();
        vogel.InsertDummy();

        while(!vogel.AllDemandSatisfied()){
            vogel.CalculatePenaltyColumn();
            vogel.CalculatePenaltyLine();
            vogel.SetMajorPenaltyValue();
            vogel.UpdateTable();
            vogel.PrintTable();

            Console.WriteLine();
            vogel.PrintDemand();
            vogel.PrintCapacity();
            Console.WriteLine();
            Console.ReadLine();
            Console.Clear();
        }

        vogel.PrintDeliveredList();
        Console.ReadLine();

    }
}
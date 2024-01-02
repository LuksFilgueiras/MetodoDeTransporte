using System.Runtime;

public class Vogel{
    public List<string> distributionCenters = new List<string>(){
        "Casa Verde",
        "Grajaú",
        "Ipiranga",
        "Itaquera",
        "Mooca",
        "Pinheiros"
    };
    public List<string> deliverLocations = new List<string>(){
        "Alto de Pinheiros",
        "Anhanguera",
        "Aricanduva",
        "Bela Vista",
        "Bom Retiro",
        "Brasilândia",
        "Cambuci",
        "Capão Redondo",
        "Freguesia do Ó",
        "Iguatemi",
        "Itaim Bibi",
        "Jabaquara",
        "Liberdade",
        "Morumbi",
        "Moema",
        "Perdizes",
        "Perus",
        "Santa Cecília",
        "Santo Amaro 01",
        "Santo Amaro 02",
        "Tatuapé",
        "Tremembé",
        "Tucuruvi",
        "Vila Mariana",
        "Vila Matilde",
    };
    public List<Route> transportRoutes = new List<Route>();

    //
    public double[,] dataTable {get; set;}
    public double[] penaltyLine {get; set;}
    public double[] penaltyColumn {get; set;}
    private Balance balanceType;

    public double[] demand {get; set;}
    public double[] capacity {get; set;}

    public bool penaltyIsColumn {get; set;}
    public double majorPenaltyValue {get; set;}

    public double cheapestValue {get; set;}

    public Vogel(double[,] dataTable){
        this.dataTable = dataTable;
        this.demand = RetrieveDemand();
        this.capacity = RetrieveCapacity();
        CleanDataTable();
        
        this.penaltyLine = new double[dataTable.GetLength(1)];
        this.penaltyColumn = new double[dataTable.GetLength(0)];
    }

    private double[] RetrieveCapacity(){
        double[] cap = new double[dataTable.GetLength(0)];

        for(int i = 0; i < cap.Length; i++){
            cap[i] = dataTable[i, dataTable.GetLength(1) - 1];
        }

        return cap;
    }   

    private double[] RetrieveDemand(){
        double[] dem = new double[dataTable.GetLength(1)];

        for(int i = 0; i < dem.Length; i++){
            dem[i] = dataTable[dataTable.GetLength(0) - 1, i];
        }

        return dem;
    }
    
    private void CleanDataTable(){
        double[,] newDataTable = new double[dataTable.GetLength(0) - 1, dataTable.GetLength(1) - 1];
        for(int i = 0; i < newDataTable.GetLength(0); i++){
            for(int j = 0; j < newDataTable.GetLength(1); j++){
                newDataTable[i, j] = dataTable[i, j];
            }
        }

        dataTable = newDataTable;
    }

    public double CalculateBalance(){
        double necSum = 0;
        double dispSum = 0;
        foreach(double dem in demand){
            necSum += dem;
        }

        foreach(double cap in capacity){
            dispSum += cap;
        }

        return dispSum - necSum;
    }

    public void SetBalanceType(){
        if(CalculateBalance() < 0){
            balanceType = Balance.capacity;
        }
        else if(CalculateBalance() > 0){
            balanceType = Balance.demand;
        }
        else{
            balanceType = Balance.balanced;
        }
    }

    public void InsertDummy(){
        if(balanceType == Balance.capacity){
            double[,] newDataTable = new double[dataTable.GetLength(0) + 1, dataTable.GetLength(1)];

            for(int i = 0; i < newDataTable.GetLength(0); i++){
                for(int j = 0; j < newDataTable.GetLength(1); j++){
                    if(i < dataTable.GetLength(0)){
                        newDataTable[i, j] = dataTable[i, j];
                    }
                }
            }

            // inserir nova linha com valor faltante

            dataTable = newDataTable;
        }
        else if(balanceType == Balance.demand){
            double[,] newDataTable = new double[dataTable.GetLength(0), dataTable.GetLength(1) + 1];

            for(int i = 0; i < newDataTable.GetLength(0); i++){
                for(int j = 0; j < newDataTable.GetLength(1); j++){
                    if(j < dataTable.GetLength(1)){
                        newDataTable[i, j] = dataTable[i, j];
                    }
                }
            }

            double[] newDemand = new double[demand.Length];

            for(int i = 0; i < newDemand.Length; i++){
                if(i < demand.Length - 1){
                    newDemand[i] = demand[i];
                }else{
                    newDemand[i] = Math.Abs(CalculateBalance());
                }
            }
            deliverLocations.Add("Dummy");
            demand = newDemand;
            dataTable = newDataTable;
        }
    }

    public void CalculatePenaltyColumn(){
        double minorValue = double.PositiveInfinity;
        double secondMinorValue = double.PositiveInfinity;

        for(int i = 0; i < dataTable.GetLength(0); i++){
            for(int j = 0; j < dataTable.GetLength(1); j++){
                if(dataTable[i, j] < minorValue && dataTable[i, j] != -1){
                    minorValue = dataTable[i, j];
                }
            }
            for(int j = 0; j < dataTable.GetLength(1); j++){
                if(dataTable[i, j] > minorValue && dataTable[i, j] < secondMinorValue && dataTable[i, j] != -1){
                    secondMinorValue = dataTable[i, j];
                }
            }
            penaltyColumn[i] = secondMinorValue - minorValue;
        }
    }

    public void CalculatePenaltyLine(){
        double minorValue = double.PositiveInfinity;
        double secondMinorValue = double.PositiveInfinity;

        for(int i = 0; i < dataTable.GetLength(1); i++){
            for(int j = 0; j < dataTable.GetLength(0); j++){
                if(dataTable[j, i] < minorValue && dataTable[j, i] != -1){
                    minorValue = dataTable[j, i];
                }
            }

            for(int j = 0; j < dataTable.GetLength(0); j++){
                if(dataTable[j, i] == 0 && minorValue == 0 && dataTable[j, i] != -1){
                    secondMinorValue = 0;
                    break;
                }
                if(dataTable[j, i] > minorValue && dataTable[j, i] < secondMinorValue && dataTable[j, i] != -1){
                    secondMinorValue = dataTable[j, i];
                }
            }

            penaltyLine[i] = secondMinorValue - minorValue;
            minorValue = double.PositiveInfinity;
            secondMinorValue = double.PositiveInfinity;
        }
    }

    public void SetMajorPenaltyValue(){
        double columnValue = 0.0;
        double lineValue = 0.0;

        foreach(double value in penaltyColumn){
            if(value > columnValue){
                columnValue = value;
            }
        }

        foreach(double value in penaltyLine){
            if(value > lineValue){
                lineValue = value;
            }
        }

        if(columnValue > lineValue){
            penaltyIsColumn = true;
            majorPenaltyValue = columnValue;
            return;
        }
            
        penaltyIsColumn = false;
        majorPenaltyValue = lineValue;
    }

    public int GetMajorPenaltyIndex(){
        if(penaltyIsColumn){
            for(int i = 0; i < penaltyColumn.Length; i++){
                if(penaltyColumn[i] == majorPenaltyValue){
                    return i;
                }
            }
        }else{
            for(int i = 0; i < penaltyLine.Length; i++){
                if(penaltyLine[i] == majorPenaltyValue){
                    return i;
                }
            }
        }
        return -1;
    }

    public int GetMinorValueIndex(){
        double minorValue = double.PositiveInfinity;
        int penaltyIndex = GetMajorPenaltyIndex();
        int minorValueLineIndex = 0;

        if(penaltyIsColumn){
            for(int i = 0; i < dataTable.GetLength(1); i++){
                if(dataTable[penaltyIndex, i] < minorValue && dataTable[penaltyIndex, i] != -1){
                    minorValue = dataTable[penaltyIndex, i];
                    cheapestValue = minorValue;
                }
            }
            for(int i = 0; i < dataTable.GetLength(1); i++){
                if(dataTable[penaltyIndex, i] == minorValue && dataTable[penaltyIndex, i] != -1){
                    minorValueLineIndex = i;
                }
            }
        }else{
            for(int i = 0; i < dataTable.GetLength(0); i++){
                if(dataTable[i, penaltyIndex] < minorValue && dataTable[i, penaltyIndex] != -1){
                    minorValue = dataTable[i, penaltyIndex];
                    cheapestValue = minorValue;
                }
            }
            for(int i = 0; i < dataTable.GetLength(0); i++){
                if(dataTable[i, penaltyIndex] == minorValue  && dataTable[i, penaltyIndex] != -1){
                    minorValueLineIndex = i;
                }
            }
        }

        return minorValueLineIndex;
    }

    public void UpdateTable(){
        int minorValueIndex = GetMinorValueIndex();
        int majorPenaltyIndex = GetMajorPenaltyIndex();

        if(penaltyIsColumn){
            double cap = capacity[majorPenaltyIndex];
            double dem = demand[minorValueIndex];

            if(cap >= dem){
                double amount = demand[minorValueIndex];
                capacity[majorPenaltyIndex] -= demand[minorValueIndex];
                demand[minorValueIndex] = 0;

                transportRoutes.Add(new Route(cheapestValue, amount, distributionCenters[majorPenaltyIndex], deliverLocations[minorValueIndex]));

                for(int i = 0; i < dataTable.GetLength(0); i++){
                    dataTable[i, minorValueIndex] = -1;
                }
            }

            if(dem > cap){
                double amount = demand[majorPenaltyIndex];
                demand[minorValueIndex] -= capacity[majorPenaltyIndex];
                capacity[majorPenaltyIndex] = 0;

                transportRoutes.Add(new Route(cheapestValue, amount, distributionCenters[majorPenaltyIndex], deliverLocations[minorValueIndex]));

                for(int i = 0; i < dataTable.GetLength(1); i++){
                    dataTable[majorPenaltyIndex, i] = -1;
                }
            }
        }
        else{
            double cap = capacity[minorValueIndex];
            double dem = demand[majorPenaltyIndex];

            if(cap >= dem){
                double amount = demand[majorPenaltyIndex];
                capacity[minorValueIndex] -= demand[majorPenaltyIndex];
                demand[majorPenaltyIndex] = 0;

                transportRoutes.Add(new Route(cheapestValue, amount, distributionCenters[minorValueIndex], deliverLocations[majorPenaltyIndex]));

                for(int i = 0; i < dataTable.GetLength(0); i++){
                    dataTable[i, majorPenaltyIndex] = -1;
                }
            }
            
            if(dem > cap){
                double amount = capacity[minorValueIndex];
                demand[majorPenaltyIndex] -= capacity[minorValueIndex];
                capacity[minorValueIndex] = 0;

                transportRoutes.Add(new Route(cheapestValue, amount, distributionCenters[minorValueIndex], deliverLocations[majorPenaltyIndex]));

                for(int i = 0; i < dataTable.GetLength(1); i++){
                    dataTable[minorValueIndex, i] = -1;
                }
            }
        }
    }

    public bool AllDemandSatisfied(){
        for(int i = 0; i < demand.Length; i++){
            if(demand[i] != 0){
                return false;
            }
        }

        return true;
    }

    public void PrintTable(){
        for(int i = 0; i < dataTable.GetLength(0); i++){
            for(int j = 0; j < dataTable.GetLength(1); j++){
                Console.Write(dataTable[i,j].ToString("00.00") + " | ");
            }
            Console.WriteLine();
        }
    }

    public void PrintDemand(){
        Console.WriteLine("DEMANDA");
        foreach(double d in demand){
            Console.Write(d + " | ");
        }
        Console.WriteLine();
        Console.WriteLine("-------------------");
    }

    public void PrintCapacity(){
        Console.WriteLine("CAPACIDADE");
        foreach(double c in capacity){
            Console.Write(c + " | ");
        }
        Console.WriteLine();
        Console.WriteLine("-------------------");
    }

    public void PrintDeliveredList(){
        foreach(Route route in transportRoutes){
            route.PrintRoute();
        }
    }
}

public enum Balance{
    balanced,
    demand,
    capacity,
}
public class Route{
    public string distributionCenter {get; set;}
    public string deliverLocation {get; set;}
    public double price {get; set;}
    public double amount {get; set;}

    public Route(double price, double amount, string distributionCenter, string deliverLocation){
        this.distributionCenter = distributionCenter;
        this.deliverLocation = deliverLocation;
        this.price = price;
        this.amount = amount;
    }

    public double CalculateTotalPrice(){
        return amount * price;
    }

    public void PrintRoute(){
        Console.WriteLine(distributionCenter + " -> " + deliverLocation);
        Console.WriteLine(price + " x " + amount + " = " + CalculateTotalPrice().ToString(".00"));
        Console.WriteLine();
        Console.WriteLine("--------------------");
    }
}
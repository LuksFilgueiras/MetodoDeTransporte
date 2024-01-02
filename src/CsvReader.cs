using System.Globalization;
using System.IO;

public class CsvReader{
    private string path;

    public CsvReader(string path){
        this.path = path;
    }

    public double[,] DataTable(){
        var reader = new StreamReader(path);
        string[] lines = reader.ReadToEnd().Split(new char[] {'\n'});

        int lineLength = lines[0].Split(',').Length;
        int linesCount = lines.Length - 1;

        double[,] table = new double[linesCount, lineLength];
        double res = 0;

        for(int i = 0; i < linesCount; i++){    
            for(int j = 0; j < lineLength; j++){
                var values = lines[i].Split(',');
                table[i, j] = double.TryParse(values[j], CultureInfo.InvariantCulture, out res) ? res : 0.0;
            }
        }

        return table;
    }

}
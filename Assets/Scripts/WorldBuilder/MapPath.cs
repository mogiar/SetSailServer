using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MapPath
{
    string theme;
    string icon;
    int x;
    int y;

    public MapPath(string name)
    {
        string[] arr = name.Split(" (".ToCharArray());
        this.theme = arr[0];
        this.icon = name.Substring(0, 1);
        this.y = (int.Parse(arr[2]))/1000;
        this.x = -1 * (int.Parse(arr[4].Substring(0, arr[4].Length - 1)))/1000;
    }
    public string Icon() { return icon; }
    public bool isEmpty() { return icon.Equals("x"); }
    public bool isNextIcon() { return !icon.Equals("x") && !icon.Equals("S"); }
    public int X() { return x; }
    public int Y() { return y; }
    public void SetTheme(string theme) { this.theme = theme; }
    public void SetIcon(string icon) { this.icon = icon; }
}


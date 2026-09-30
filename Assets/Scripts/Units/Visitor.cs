using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Visitor
{
    float Attack(SharkScript shark);
    float Attack(FishScript Fish);
    float Attack(PlayerStats player);
}


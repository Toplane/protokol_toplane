using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolApp.Klase
{
    public class DaysBack
    {
        public double BrojDana { get; set; }
        public string Naziv { get; set; }

    }


    public class DaysBackList
    {
        public  List<DaysBack> daysBacks = new List<DaysBack>();

        public DaysBackList()
        {
            daysBacks.Add(SedamDana);
            daysBacks.Add(PetnaestDana);
            daysBacks.Add(MjesecDana);
            daysBacks.Add(TriMjeseca);
            daysBacks.Add(Sve);

        }

        private readonly DaysBack SedamDana = new DaysBack() {BrojDana=-7, Naziv="Sedam dana"};
        private readonly DaysBack PetnaestDana = new DaysBack() { BrojDana = -15, Naziv = "Petnaest dana" };
        private readonly DaysBack MjesecDana = new DaysBack() { BrojDana = -30, Naziv = "Mjesec dana" };
        private readonly DaysBack TriMjeseca = new DaysBack() { BrojDana = -90, Naziv = "Tri mjeseca" };
        private readonly DaysBack Sve = new DaysBack() { BrojDana = 0, Naziv = "Prikaži sve" };


    }

    
}

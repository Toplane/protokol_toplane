using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using Telerik.WinControls;
using Telerik.WinControls.Export;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Export;

namespace ProtokolApp
{
    

    class Helper
    {
        private static string konekcija = Directory.GetCurrentDirectory() + "\\konekcija";

        public static List<sluzbe> GetSluzbeZaKorisnika(int korisnikId)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                var kor = context.korisnik.Where(k => k.ID == korisnikId).FirstOrDefault();

            //   var slu = context.sluzbe.Where(sl => sl.korisnik1.All(kori => sl.korisnik1.Contains(kori))).ToList();

                var slu = (from slz in context.sluzbe
                    where slz.korisnik1.Any(c => c.ID == kor.ID)
                    select slz).ToList();
                return slu;


               
            }
        

    }

        public static int BrojDokumenata(int idProtokola)
        {
            int broj = 0;
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                broj = context.dokument.Where(d => d.ID_protokola == idProtokola && d.Izbrisan==0).Count();
                return broj;
            }
        }
        public static int GetIDKorisnika(string korisnickoime)
        {
            int idKorisnika = 0;
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    idKorisnika = context.korisnik.Where(k => k.Naziv == korisnickoime).Select(k => k.ID)
                        .FirstOrDefault();
                    
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            

            return idKorisnika;
        }

        public static bool CanEdit(string korisnickoime)
        {
            int canEdit = 0;
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    canEdit = (int) context.korisnik.Where(k => k.Naziv == korisnickoime).Select(k => k.canedit)
                        .FirstOrDefault();
                }
            }
            catch
            {
                return false;
            }


            return canEdit != 0 ? true : false;
        }
        public static bool CanNew(string korisnickoime)
        {
            int canNew = 0;
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    canNew = (int)context.korisnik.Where(k => k.Naziv == korisnickoime).Select(k => k.cannew)
                        .FirstOrDefault();
                }
            }
            catch
            {
                return false;
            }


            return canNew != 0 ? true : false;
        }
        public static bool CanInsertNewFile(string korisnickoime)
        {
            int canInsertNewFile = 0;
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    canInsertNewFile = (int)context.korisnik.Where(k => k.Naziv == korisnickoime).Select(k => k.caninsertfile)
                        .FirstOrDefault();
                }
            }
            catch
            {
                return false;
            }


            return canInsertNewFile != 0 ? true : false;
        }

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork && ipAddress.ToString().StartsWith("192"))
                    return ipAddress.ToString();
            }

            return string.Empty;
        }

        public static List<int> GetGodineProtokola(int idSluzbe)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                List<int> godine = context.protokol.Where(k => k.ID_sluzbe == idSluzbe).Select(k => k.datum.Value.Year)
                    .Distinct().ToList();
                if (!godine.Any())
                {
                    godine.Add(DateTime.Now.Year);
                }
                    
                return godine;
            }
        }

        public static int GetIDSluzbe(string korisnickoime)
        {
            int idSluzbe = 0;
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                
                idSluzbe = context.korisnik.Where(k => k.Naziv == korisnickoime).Select(k => k.ID_sluzbe)
                    .FirstOrDefault();
            }

            return idSluzbe;

        }

        public static string GetNazivSluzbe(int idKorisnika)
        {
            string sluzba;
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                sluzba = context.korisnik.Where(k => k.ID == idKorisnika).Select(k => k.sluzbe.Naziv).FirstOrDefault();
            }

            return sluzba;
        }

        public static int GetNextMaxBrojProtokola(int sluzba, int year)
        {
            int broj = 0;
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    broj = (int) context.protokol.Where(p =>
                            p.datum.Value.Year == year && p.ID_sluzbe == sluzba && p.izbrisan == 0)
                        .Select(p => p.redni_broj).Max();
                    
                }
            }
            catch (Exception ex)
            {
                //ne postoji broj
            }

            return broj+1;
        }

//        public static bool UnosProtokoluBazuSP(int rednibroj, int tipDokumentaUlazIzlaz, int idSluzbe, string tipProtokola_SP,
//            string brojUlaznogProtokola_SP, DateTime datumPrijema_SP, string signirano_SP, string vrstaDokumenta_sp, string sifraDokumenta_SP,
//           string nacinPrijema_SP, string lokacijaCZK_SP, string sifraProstora_SP, string nazivKorisnikaUsluge_SP, string adresaProstora_SP, 
//            DateTime rokZaOdgovor_SP, string brojRjesenja_SP, string vrstaRjesenja_SP, string pripremioRjesenje_SP, decimal iznosPoRjesenju_SP,
//            string brojIzlaznogProtokola_SP, DateTime datumOdgovora_SP, string statusReklamacije_SP, string putanjaPredmet, string nazivPredmeta, string putanjaVeza, string veza)
//        {
//            try
//            {
//                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
//                {
//                    protokol p = new protokol();

//                    var slu = context.sluzbe.Where(s => s.ID == idSluzbe).Select(s => s).FirstOrDefault();
//                    p.sluzbe = slu;

//                    p.redni_broj = rednibroj;
//                    p.ID_tipa = tipDokumentaUlazIzlaz;
//                    p.datum = datumPrijema_SP;
//                    p.datumPrijema_SP = datumPrijema_SP;
//                    p.predmet = nazivPredmeta;
//                    p.tipProtokola_SP = tipProtokola_SP;
//                    p.brojUlaznogProtokola_SP = brojUlaznogProtokola_SP;
//                    p.datumPrijema_SP = datumPrijema_SP;
//                    p.signirano_SP = signirano_SP;
//                    p.vrstaDokumenta_sp = vrstaDokumenta_sp;
//                    p.sifraDokumenta_SP = sifraDokumenta_SP;
//                    p.nacinPrijema_SP = nacinPrijema_SP;
//                    p.lokacijaCZK_SP = lokacijaCZK_SP;
//                    p.sifraProstora_SP = sifraProstora_SP;
//                    p.nazivKorisnikaUsluge_SP = nazivKorisnikaUsluge_SP;
//                    p.adresaProstora_SP = adresaProstora_SP;
//                    p.rokZaOdgovor_SP = rokZaOdgovor_SP;
//                    p.brojRjesenja_SP = brojRjesenja_SP;
//                    p.vrstaRjesenja_SP = vrstaRjesenja_SP;
//                    p.pripremioRjesenje_SP = pripremioRjesenje_SP;
//                    p.iznosPoRjesenju_SP = iznosPoRjesenju_SP;
//                    p.brojIzlaznogProtokola_SP = brojIzlaznogProtokola_SP;
//                    p.datumOdgovora_SP = datumOdgovora_SP;
//                    p.statusReklamacije_SP = statusReklamacije_SP;
//                    p.Racunar = Environment.MachineName;


//                    if (putanjaPredmet.EndsWith(".pdf"))
//                    {
//                        dokument d = new dokument { TipDokumenta = 1, Dokument = File.ReadAllBytes(putanjaPredmet) };
//                        d.Filename = putanjaPredmet.Substring(putanjaPredmet.LastIndexOf('\\') + 1);
//                        d.Izbrisan = 0;
//                        d.Opis = nazivPredmeta;
//                        p.dokument.Add(d);
//                    }

//                    if (putanjaVeza.EndsWith(".pdf"))
//                    {
//                        dokument d = new dokument { TipDokumenta = 2, Dokument = File.ReadAllBytes(putanjaVeza) };
//                        d.Filename = putanjaVeza.Substring(putanjaVeza.LastIndexOf('\\') + 1);
//                        d.Izbrisan = 0;
//                        d.Opis = veza;
//                        p.dokument.Add(d);
//                    }

//                    context.protokol.Add(p);
//                    context.SaveChanges();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//#if DEBUG
//                MessageBox.Show(ex.ToString());
//#endif
//                return false;
//            }
//        }

        public static bool UnosProtokolUBazu(int idSluzbe, int rednibroj, int tipDokumentaUlazIzlaz, DateTime datum, string nazivPredmeta, string veza,
    string putanjaPredmet, string putanjaVeza, string oznakaRegistratora, string arhiva, string oznakaDopisa, string dostavaDopisa, string napomena, string razvod)
        {
            try
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    protokol p = new protokol();

                    //var slu = context.sluzbe.Where(s => s.ID == idSluzbe).Select(s => s).FirstOrDefault();
                   // p.sluzbe = slu;

                    p.ID_sluzbe = idSluzbe;
                    p.redni_broj = rednibroj;
                    p.ID_tipa = tipDokumentaUlazIzlaz;
                    p.datum = datum;
                    p.predmet = nazivPredmeta;
                    p.veza = veza;
                    p.oznaka_dopisa = oznakaDopisa;
                    p.oznaka_registratora = oznakaRegistratora;
                    p.dostava_dopisa = dostavaDopisa;
                    p.arhiva = arhiva;
                    p.napomena = napomena;
                    p.razvod = razvod;
                    p.Racunar = Environment.MachineName;
                    p.DatumVrijeme=DateTime.Now;
                    

                    if (putanjaPredmet.EndsWith(".pdf"))
                    {
                        dokument d = new dokument {TipDokumenta = 1, Dokument = File.ReadAllBytes(putanjaPredmet)};
                        d.Filename = putanjaPredmet.Substring(putanjaPredmet.LastIndexOf('\\')+1);
                        d.Izbrisan = 0;
                        d.Opis = nazivPredmeta;
                        d.Racunar = Environment.MachineName;
                        d.DatumUnosa = DateTime.Now;
                        d.DatumVrijeme = DateTime.Now;
                        p.dokument.Add(d);
                        
                    }

                    if (putanjaVeza.EndsWith(".pdf"))
                    {
                        dokument d = new dokument {TipDokumenta = 2, Dokument = File.ReadAllBytes(putanjaVeza)};
                        d.Filename = putanjaVeza.Substring(putanjaVeza.LastIndexOf('\\') + 1);
                        d.Izbrisan = 0;
                        d.Opis = veza;
                        d.Racunar = Environment.MachineName;
                        d.DatumUnosa = DateTime.Now;
                        d.DatumVrijeme = DateTime.Now;
                        p.dokument.Add(d);
                    }

                    context.protokol.Add(p);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

                return false;
            }

        }



        public static void ChangeMouseOverState(RadButton target, int i)
        {

            if (i == 1)
            {
                target.ButtonElement.IsMouseOver = true;
            }
            else
            {
                target.ButtonElement.IsMouseOver = false;
            }

        }

        public static protokol GetSingle(int idProtokola)
        {
            using (var context = new protokolEntities1(ProcitajEntityConnectionString()))
            {
                var p = context.protokol.Find(idProtokola);
                return p;
            }
        }
//        public static bool UnosProtokolUBazuSP(int idSluzbe, int rednibroj, string tipProtokolaSP, string brojUlaznogProtokola_SP, 
//            string signirano_SP,  
//            DateTime datumPrijemaSP, 
//            string vrstaDokumenta_sp, 
//            string sifraDokumenta_SP,
//            string nacinPrijema_SP, 
//            string putanjaVeza, 
//            string lokacijaCZK_SP, 
//            string sifraProstora_SP, 
//            string nazivKorisnikaUsluge_SP, 
//            string adresaProstora_SP, 
//            DateTime rokZaOdgovor_SP,
//            string brojRjesenja_SP,
//            string vrstaRjesenja_SP,
//            string pripremioRjesenje_SP,
//            decimal iznosPoRjesenju_SP,
//            string brojIzlaznogProtokola_SP,
//            DateTime datumOdgovora_SP,
//            string statusReklamacije_SP,
//            string napomena,
//            string putanjaPredmet,
//            string veza
//            )
//        {
//            try
//            {
//                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
//                {
//                    protokol p = new protokol();

//                    var slu = context.sluzbe.Where(s => s.ID == idSluzbe).Select(s => s).FirstOrDefault();
//                    p.sluzbe = slu;

//                    p.redni_broj = rednibroj;
//                    p.ID_tipa = 0;
//                    p.datumPrijema_SP = datumPrijemaSP;
//                    p.signirano_SP = signirano_SP;
//                    p.vrstaDokumenta_sp = vrstaDokumenta_sp;
//                    p.sifraDokumenta_SP = sifraDokumenta_SP;
//                    p.nacinPrijema_SP = nacinPrijema_SP;
//                    p.lokacijaCZK_SP = lokacijaCZK_SP;
//                    p.sifraProstora_SP = sifraProstora_SP;
//                    p.nazivKorisnikaUsluge_SP = nazivKorisnikaUsluge_SP;
//                    p.adresaProstora_SP = adresaProstora_SP;
//                    p.rokZaOdgovor_SP = rokZaOdgovor_SP;
//                    p.brojRjesenja_SP = brojRjesenja_SP;
//                    p.vrstaRjesenja_SP = vrstaRjesenja_SP;
//                    p.pripremioRjesenje_SP = pripremioRjesenje_SP;
//                    p.iznosPoRjesenju_SP = iznosPoRjesenju_SP;
//                    p.brojIzlaznogProtokola_SP = brojIzlaznogProtokola_SP;
//                    p.datumOdgovora_SP = datumOdgovora_SP;
//                    p.statusReklamacije_SP = statusReklamacije_SP;


//                    if (putanjaPredmet.EndsWith(".pdf"))
//                    {
//                        dokument d = new dokument { TipDokumenta = 1, Dokument = File.ReadAllBytes(putanjaPredmet) };
//                        d.Filename = putanjaPredmet.Substring(putanjaPredmet.LastIndexOf('\\') + 1);
//                        d.Izbrisan = 0;
//                        d.Opis = sifraDokumenta_SP;
//                        p.dokument.Add(d);
//                    }

//                    if (putanjaVeza.EndsWith(".pdf"))
//                    {
//                        dokument d = new dokument { TipDokumenta = 2, Dokument = File.ReadAllBytes(putanjaVeza) };
//                        d.Filename = putanjaVeza.Substring(putanjaVeza.LastIndexOf('\\') + 1);
//                        d.Izbrisan = 0;
//                        d.Opis = veza;
//                        p.dokument.Add(d);
//                    }

//                    context.protokol.Add(p);
//                    context.SaveChanges();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//#if DEBUG
//                MessageBox.Show(ex.ToString());
//#endif
//                return false;
//            }

//        }



        public static string ProcitajEntityConnectionString()
        {
            EntityConnectionString con = EntityConnectionString.CitajEntityConnectionString(konekcija);
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();

                entityConnectionStringBuilder.Provider = con.Provider;
                entityConnectionStringBuilder.ProviderConnectionString = con.ProviderConnectionString;
                entityConnectionStringBuilder.Metadata = con.Metadata;
                return entityConnectionStringBuilder.ToString();
        }

        public static string ProcitajConnectionString()
        {
            EntityConnectionString con = EntityConnectionString.CitajEntityConnectionString(konekcija);
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();

            entityConnectionStringBuilder.Provider = con.Provider;
            entityConnectionStringBuilder.ProviderConnectionString = con.ProviderConnectionString;
            entityConnectionStringBuilder.Metadata = con.Metadata;
            return con.ProviderConnectionString;
        }


    }

    [Serializable]
    public class EntityConnectionString
    {
        public string Provider { get; set; }
        public string ProviderConnectionString { get; set; }
        public string Metadata { get; set; }

        public static bool SacuvajEntityConnectionString(string user, string passsword, string server, string baza, string putanjaFileName)
        {
            try
            {
                EntityConnectionString connectionString = new EntityConnectionString();
                connectionString.Metadata =
                    Encrypt("res://*/protokolEntities.csdl|res://*/protokolEntities.ssdl|res://*/protokolEntities.msl");
                connectionString.Provider = Encrypt("System.Data.SqlClient");
                connectionString.ProviderConnectionString = Encrypt("Data Source=" + server + ";Initial Catalog=" +
                                                                    baza +
                                                                    ";Persist Security Info=True;User ID=" + user +
                                                                    ";" +
                                                                    "Password" + "=" + passsword + ";");

                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(putanjaFileName, FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(stream, connectionString);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static EntityConnectionString CitajEntityConnectionString(string fileName)
        {
            try
            {
                EntityConnectionString connectionString = new EntityConnectionString();

                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    connectionString = (EntityConnectionString) formatter.Deserialize(stream);
                    connectionString.Metadata = Decrypt(connectionString.Metadata);
                    connectionString.Provider = Decrypt(connectionString.Provider);
                    connectionString.ProviderConnectionString = Decrypt(connectionString.ProviderConnectionString);
                    return connectionString;
                }
            }
            catch
            {
                
                throw new Exception("Ne mogu ucitati file!");
            }


        }
        private static string Decrypt(string cipherText)
        {
            string EncryptionKey = "pwd1122";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4f, 0x88, 0x64, 0x56, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "pwd1122";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4f, 0x88, 0x64, 0x56, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}

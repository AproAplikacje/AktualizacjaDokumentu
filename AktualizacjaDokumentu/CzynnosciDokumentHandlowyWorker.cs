using AktualizacjaDokumentu;
using Soneta.Business;
using Soneta.Handel;
using Soneta.Towary.Skladniki;
using static Soneta.Towary.Skladniki.SkladnikiProduktuWorker;

[assembly: Worker(typeof(CzynnosciDokumentHandlowyWorker), typeof(DokumentHandlowy))]

namespace AktualizacjaDokumentu
{
    public class CzynnosciDokumentHandlowyWorker : ContextBase
    {
        public CzynnosciDokumentHandlowyWorker(Context context) : base(context) { }

        [Context]
        public DokumentHandlowy Handlowy
        {
            get; set;
        }

        public void ZamienPozycjeNaSkladniki(PozycjaDokHandlowego pozycja)
        {
            var skladnikiWorker = new SkladnikiProduktuWorker(Context)
            {
                Pozycja = pozycja
            };

            QueryContextInformation query = (QueryContextInformation)skladnikiWorker.WybierzSkladniki();
            var data = (SkladnikiProduktu)query.Data;

            using (ITransaction transaction = base.Session.Logout(editMode: true))
            {
                var pozycjaNadrzednaZo = pozycja.Nadrzędne["Zamówienie od odbiorcy"]?.Pozycja;

                foreach (SkladnikProduktu item in data.ElementyKompletu)
                {
                    PozycjaDokHandlowego pozycjaDokHandlowego = new PozycjaDokHandlowego(pozycja.Dokument);
                    pozycja.Table.AddRow(pozycjaDokHandlowego);
                    pozycjaDokHandlowego.Towar = item.ElementKompletu;
                    pozycjaDokHandlowego.Ilosc = item.Ilosc / pozycja.Towar.Produkt.Ilosc.Value;

                    /*
                    // to muszę ustawiać, jeśli chcę zachować informację o ilości z kompletów
                    if (pozycjaNadrzednaZo != null)
                        pozycjaDokHandlowego.Features["ZKompletu"] = pozycja.Towar.Kod + $@" ( {pozycjaNadrzednaZo.Ilosc * item.IloscBazowa} )";
                    */
                }

                pozycja.Delete();
                transaction.CommitUI();
            }
        }
    }
}

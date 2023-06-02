using AktualizacjaDokumentu;
using Microsoft.Extensions.DependencyInjection;
using Soneta.Business;
using Soneta.Handel;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Kasa.Extensions;
using System;
using System.Linq;

[assembly: Worker(typeof(ZamianaPozycjiDokumentu), typeof(DokumentHandlowy))]

namespace AktualizacjaDokumentu
{
    public class ZamianaPozycjiDokumentu : ContextBase
    {
        public ZamianaPozycjiDokumentu(Context context) : base(context) { }

        [Context]
        public DokumentHandlowy Dokument { get; set; }

        /*
        W konfiguracji jest utworzony podrzędny dokument przyjęcia magazynowego
        można założyć, że jest to dokument PW

        Na dokument PW są kopiowane komplety, które są rozbijane na składniki,

        Składnikiami kompletu są inne komplety.

        Potrzbuję w momencie utworzenia dokument PW rozbić pozycję na składniki

        Najlepiej zgyby to zrobić przed momentem wyświetlenia operatorowu dokuemntu PW

        Nie udało mi się zrobić tego za pomocą ITworzenieDokumentuHandlowego, więc musiałem zrobić to za pomocą Workera
        który wywoływany jest bezpośrednio z dokumentu ZO

        Czy jest możliwość wbicia się w moment gdy jest utworzony dokument PW i przed wyświetleniem go operatorowi?
        */

        [Action("Generuj podrzędny dla zestawów", Mode = ActionMode.SingleSession)]
        public object GenerujPodrzedny()
        {
            return GenerujPrzyjecieDlaZo("PW");
        }

        object GenerujPrzyjecieDlaZo(string symbolDokumentu)
        {
            var relationsApi = Dokument.Session.GetRequiredService<IRelacjeService>();

            var wydanie = relationsApi
               .NowyPodrzednyIndywidualny(
                new[] { Dokument },
                symbolDokumentu,
                handlers: new HandlerSet
                {
                    WybierzPozycjeCallback = WybierzPozycje
                });

            var dok = wydanie[0];
            dok.Pozycje.ForEachElement(pozycja =>
            {
                new CzynnosciDokumentHandlowyWorker(Context).ZamienPozycjeNaSkladniki(pozycja);
            });

            return wydanie[0];
        }

        private void WybierzPozycje(DokumentDocelowy docelowy)
        {
            var wybor = new RelacjaPozycjiWorker { Docelowy = docelowy };
            foreach (var p in docelowy.Pozycje.Cast<PozycjaDokHandlowego>())
            {
                wybor.Pozycja = p;
                wybor.Kopiuj = true;
                wybor.Ilość = wybor.Ilość;
            }
        }
    }
}

using AktualizacjaDokumentu;
using Soneta.Business;
using Soneta.Handel;

[assembly: Service(typeof(ITworzenieDokumentuHandlowego), typeof(TworzenieDokumentuHandlowego), ServiceScope.Session)]

namespace AktualizacjaDokumentu
{
    public class TworzenieDokumentuHandlowego : ITworzenieDokumentuHandlowego
    {
        /*
        Potrzebuję mechanizm do aktualizacji dokumentu po jego utworzeniu z relacji
        a przed wyświetleniem otwartego formularza dokumentu operatorowi.
         */
        public void DodawaniePozycji(TworzenieDokumentuHandlowegoArgs args)
        {

        }

        public void DokumentUtworzony(TworzenieDokumentuHandlowegoArgs args)
        {
            // DokumentUtworzony - nie ma jeszcze pozycji
        }

        public void PozycjaDodana(TworzenieDokumentuHandlowegoArgs args)
        {
            //niestety args.Pozycja nie ma jeszcze przypisanego Towaru
        }

        public void TworzenieDokumentu(TworzenieDokumentuHandlowegoArgs args)
        {

        }
    }
}

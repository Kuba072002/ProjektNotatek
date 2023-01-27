# ProjektNotatek
## Aplikacja internetowa do przechowywania notatek
Jest to aplikacja umozliwiajaca przechowywanie notatek ,które można tez zaszyfrowac za pomoca hasla.
Mozliwe jest udostepnianie notatek (nieszyfrowanych) innym uzytkownik oraz ustawienie jej jako publicznej,
wtedy kazdy zalogowany uzytkownik bedzie mial do niej dostep
## Wymagania
Aby urochomic aplikacje nalezy miac zainstolowanego dockera
## Urochomienie aplikacji
Nalezy skolonowac repozytoeium metoda 
<code> git clone https://github.com/Kuba072002/ProjektNotatek.git </code>
<br />Nastepnie przejsc do folderu ProjektNotatek <code>cd ProjektNotatek</code>
<br />Zbudowac obraz za pomoca komedy: <code>docker-compose build</code>
<br />Uruchomić kontener za pomocą komendy: <code>docker-compose up</code>
<br />Dostęp do aplikacji będziemy mieli na porcie 7210: <code>https://localhost:7210/</code>
<br />Jesli jednak program sie nie urochomi nalezy odpalic go przez visual studio

##Technologie
Aplikacja została wykonana przy pomocy asp.net core Web app 
<br /> .net 7.0
<br /> Baza danych Postgres
<br /> Asp.net core Identity -sytem pozwalajacy na zarzadzaniu uzytkownikami


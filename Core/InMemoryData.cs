using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Core
{
    public static class InMemoryData
    {

        public static List<User> Users = new List<User>()
        {
            new User() { UserId = 1, Username = "TestUser", Email = "test@user.pl", GrupyObiektow = new List<GrupaObiektow>(),
                EncodedPassword = Convert.ToBase64String("TestPass321".Select(Convert.ToByte).ToArray())}
        };

        public static List<TypParametrow> TypyParametrow = new List<TypParametrow>()
        {
            new TypParametrow() {TypParametrowId = 1, Nazwa = "Wysokość", Symbol = "H", TypDanych = "FLOAT", JednostkaMiary = "m"},
            new TypParametrow() {TypParametrowId = 2, Nazwa = "Długość", Symbol = "L", TypDanych = "FLOAT", JednostkaMiary = "m"},
            new TypParametrow() {TypParametrowId = 3, Nazwa = "Szerokość", Symbol = "W", TypDanych = "FLOAT", JednostkaMiary = "m"},
            new TypParametrow() {TypParametrowId = 4, Nazwa = "Moc", Symbol = "P", TypDanych = "INT", JednostkaMiary = "dbm"},
            new TypParametrow() {TypParametrowId = 5, Nazwa = "Status", Symbol = "ST", TypDanych = "ENUM", AkceptowalneWartosci = new []
            {
                "W BUDOWIE",
                "PROJEKT",
                "PRZETARG",
                "UKOŃCZONY"
            }},
        };

        public static List<GrupaObiektow> Grupy = new List<GrupaObiektow>()
        {
            new GrupaObiektow() {GrupaObiektowId = 1, Nazwa = "Nadajniki BTS", Symbol = "BTS", Obiekty = new List<Obiekt>(),
                TypyParametrow = new List<TypParametrow>()
                {
                    TypyParametrow.First(t => t.Symbol == "H"),
                    TypyParametrow.First(t => t.Symbol == "P"),
                    TypyParametrow.First(t => t.Symbol == "ST"),
                }},
            new GrupaObiektow() {GrupaObiektowId = 2, Nazwa = "Autostrady", Symbol = "A", Obiekty = new List<Obiekt>(),
                TypyParametrow = new List<TypParametrow>()
                {
                    TypyParametrow.First(t => t.Symbol == "L"),
                    TypyParametrow.First(t => t.Symbol == "ST"),
                }}
        };

        public static List<Obiekt> Obiekty = new List<Obiekt>()
        {
            new Obiekt()
            {
                ObiektId = 1, GrupaObiektowId = 1, UserId = 1,
                Latitude = 20.151515M, Longitude = 22.232323M, Nazwa = "BTS TMobile Warszawa", Symbol = "BTS-WAW-1",
                Parametry = new List<Parametr>() {
                    new Parametr() { ObiektId = 1, ParametrId = 1, Wartosc = "25", TypParametrowId = 1},
                    new Parametr() { ObiektId = 1, ParametrId = 2, Wartosc = "35", TypParametrowId = 4},
                    new Parametr() { ObiektId = 1, ParametrId = 3, Wartosc = "PROJEKT", TypParametrowId = 5}
                }
            },
            new Obiekt()
            {
                ObiektId = 2, GrupaObiektowId = 1, UserId = 1,
                Latitude = 23.151515M, Longitude = 21.232323M, Nazwa = "BTS TMobile Wrocław", Symbol = "BTS-WRO-1",
                Parametry = new List<Parametr>() {
                    new Parametr() { ObiektId = 2, ParametrId = 4, Wartosc = "23", TypParametrowId = 1},
                    new Parametr() { ObiektId = 2, ParametrId = 5, Wartosc = "31", TypParametrowId = 4},
                    new Parametr() { ObiektId = 2, ParametrId = 6, Wartosc = "UKOŃCZONY", TypParametrowId = 5}
                }
            },
            new Obiekt()
            {
                ObiektId = 3, GrupaObiektowId = 2, UserId = 1,
                Latitude = 15.151515M, Longitude = 16.232323M, Nazwa = "Autostrada A4", Symbol = "A4",
                Parametry = new List<Parametr>() {
                    new Parametr() { ObiektId = 3, ParametrId = 7, Wartosc = "231540", TypParametrowId = 2},
                    new Parametr() { ObiektId = 3, ParametrId = 8, Wartosc = "W BUDOWIE", TypParametrowId = 5}
                }
            },
            new Obiekt()
            {
                ObiektId = 4, GrupaObiektowId = 2, UserId = 1,
                Latitude = 25.151515M, Longitude = 18.232323M, Nazwa = "Autostrada A3", Symbol = "A3",
                Parametry = new List<Parametr>() {
                    new Parametr() { ObiektId = 4, ParametrId = 9, Wartosc = "147800", TypParametrowId = 2},
                    new Parametr() { ObiektId = 4, ParametrId = 10, Wartosc = "PRZETARG", TypParametrowId = 5}
                }
            }
        };
    }
}
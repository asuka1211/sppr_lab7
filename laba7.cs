

using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Map;
using OSMLSGlobalLibrary.Modules;
using System;
using System.Collections;
using System.Collections.Generic;

namespace laba7
{
    public class Laba7 : OSMLSModule
    {
        Controller controller;
        protected override void Initialize()
        {
            controller = new Controller();
            // Создание координат полигона.
            var polygonCoordinates = new Coordinate[] {
                 new Coordinate(4817367, 6144314),
                 new Coordinate(4817367, 6267072),
                 new Coordinate(4950673, 6267072),
                 new Coordinate(4950673, 6144314),
                 new Coordinate(4817367, 6144314)
            };
            // Создание стандартного полигона по ранее созданным координатам.
            controller.tempList.Add(new Temperature(new LinearRing(polygonCoordinates)));

            var polygonCoordinates2 = new Coordinate[] {
                  new Coordinate(4950673, 6144314),
                 new Coordinate(4950673, 6267072),
                 new Coordinate(5040869, 6267072),
                 new Coordinate(5040869, 6144314),
                 new Coordinate(4950673, 6144314)
            };
            // Создание стандартного полигона по ранее созданным координатам.
            controller.tempList.Add(new Temperature(new LinearRing(polygonCoordinates2)));



            MapObjects.Add(controller.tempList[0]);
            MapObjects.Add(controller.tempList[1]);

        }


        public override void Update(long elapsedMilliseconds)
        {
            controller.changeTemperature();
            controller.checkLife();
            foreach (Temperature it in controller.tempList)
            {

                foreach (Rain rain in new List<Rain>(it.rains))
                {
                    if (rain.life <= 0)
                    {
                        MapObjects.Remove(rain);
                        it.rains.Remove(rain);
                    }
                    if (it.temperature < 0)
                    {
                        Ice ice = new Ice(rain.Coordinate);

                        MapObjects.Remove(rain);
                        it.rains.Remove(rain);

                        it.ices.Add(ice);
                        MapObjects.Add(ice);
                    }
                }
                foreach (Hail hail in new List<Hail>(it.hails))
                {
                    if (hail.life <= 0)
                    {
                        Rain rain = new Rain(hail.Coordinate);

                        MapObjects.Remove(hail);
                        it.hails.Remove(hail);

                        it.rains.Add(rain);
                        MapObjects.Add(rain);
                    }

                }
                foreach (Snow snow in new List<Snow>(it.snows))
                {
                    if (snow.life <= 0)
                    {
                        Rain rain = new Rain(snow.Coordinate);

                        MapObjects.Remove(snow);
                        it.snows.Remove(snow);

                        it.rains.Add(rain);
                        MapObjects.Add(rain);
                    }
                }
                foreach (Ice ice in new List<Ice>(it.ices))
                {
                    if (ice.life <= 0)
                    {
                        Rain rain = new Rain(ice.Coordinate);

                        MapObjects.Remove(ice);
                        it.ices.Remove(ice);

                        it.rains.Add(rain);
                        MapObjects.Add(rain);
                    }
                }
            }

            controller.createPrecipitation();

            foreach (Temperature it in controller.tempList)
            {
                foreach (Hail hail in it.hails)
                {
                    MapObjects.Add(hail);
                }
                foreach (Rain rain in it.rains)
                {
                    MapObjects.Add(rain);
                }
                foreach (Snow snow in it.snows)
                {
                    MapObjects.Add(snow);
                }
            }

            controller.printData();
        }
    }


    class Temperature : Polygon
    {
        public double temperature { set; get; }
        public List<Rain> rains = new List<Rain>();
        public List<Hail> hails = new List<Hail>();
        public List<Snow> snows = new List<Snow>();
        public List<Ice> ices = new List<Ice>();

        public Temperature(LinearRing linearRing) : base(linearRing)
        {
        }

        /// <summary>
        /// Двигает самолет вверх-вправо.
        /// </summary>
        public void heat()
        {
            if (temperature < 30)
            {
                temperature++;
            }
        }

        public void cold()
        {
            if (temperature > -30)
            {
                temperature--;
            }
        }

        public void checkPrecipitationLife()
        {
            foreach (Hail hail in hails)
            {
                if (temperature > 0)
                {
                    hail.life--;
                }
            }
            foreach (Rain rain in rains)
            {
                if (temperature > 0)
                {
                    rain.life--;
                }
            }
            foreach (Snow snow in snows)
            {
                if (temperature > 0)
                {
                    snow.life--;
                }
            }
            foreach (Ice ice in ices)
            {
                if (temperature > 0)
                {
                    ice.life--;
                }
            }
        }

    }

    [CustomStyle(
      @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(0, 0, 255, 0.4)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Rain : Point
    {
        public int life = 25;
        public Rain(Coordinate coordinate) : base(coordinate)
        {
        }
    }

    [CustomStyle(
       @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(128, 0, 0, 0.4)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Hail : Point
    {
        public int life = 3;

        public Hail(Coordinate coordinate) : base(coordinate)
        {
        }

    }

    [CustomStyle(
      @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(255, 200, 255, 0.4)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Snow : Point
    {
        public int life = 10;

        public Snow(Coordinate coordinate) : base(coordinate)
        {
        }

    }

    [CustomStyle(
     @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(0, 191, 255, 1)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Ice : Point
    {
        public int life = 7;

        public Ice(Coordinate coordinate) : base(coordinate)
        {
        }

    }

    class Controller
    {
        public List<Temperature> tempList = new List<Temperature>();
        public Random random = new Random();

        public void changeTemperature()
        {
            foreach (Temperature it in tempList)
            {
                if (random.Next(0, 10) % 2 == 0)
                {
                    it.heat();
                }
                else
                {
                    it.cold();
                }
            }
        }

        public void createPrecipitation()
        {
            foreach (Temperature it in tempList)
            {
                var check = random.Next(0, 10);
                if (it.temperature > 0)
                {
                    if (check == 5)
                    {
                        it.hails.Add(new Hail(generateCoordinate(it)));
                    }
                    else if (check % 2 == 0)
                    {
                        it.rains.Add(new Rain(generateCoordinate(it)));
                    }
                }
                else
                {
                    if (check % 2 == 0)
                    {
                        it.snows.Add(new Snow(generateCoordinate(it)));
                    }
                }
            }
        }

        public void checkLife()
        {
            foreach (Temperature it in tempList)
            {
                it.checkPrecipitationLife();
            }
        }

        private Coordinate generateCoordinate(Temperature temperature)
        {
            Coordinate[] cord = temperature.Coordinates;
            var answer = new Coordinate(random.Next((int)cord[0].X, (int)cord[2].X), random.Next((int)cord[0].Y, (int)cord[2].Y));
            return answer;
        }

        public void printData()
        {
            Console.WriteLine("1 полигон температура: " + tempList[0].temperature);
            Console.WriteLine("1 полигон кол-во градов: " + tempList[0].hails.Count);
            Console.WriteLine("2 полигон температура: " + tempList[1].temperature);
        }
    }
}

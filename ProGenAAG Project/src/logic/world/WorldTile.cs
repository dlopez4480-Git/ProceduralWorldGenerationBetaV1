using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProGenAAG_Project
{

    public class WorldTile {
        public String locationID {  get; set; }
        public String locationName { get; set; }
        public Coords coordinates { get; set; }
        

        //  Terrain Values
        public String landType { get; set; }     //  WATER, LAND, MOUNTAIN
        public int elevation { get; set; }          // RANGE:   0 - 63


        //  Biome/Climate Values
        public String climateType { get; set; }     //  FREEZING, COLD, TEMPERATE, WARM, HOT : ARID, DRY, 
        public int temperature { get; set; }
        public int hydration { get; set; }         //   0-100
        
        
        public String terrainType { get; set; }
        public String terrainFeatures { get; set; }

        







        public Bitmap terrainTypeBitmap { get; set; }
        public String terrainTypeBitmapString { get; set; }



        public Boolean[] riverBorders { get; set; }  //  Direction-TRUE:     North-0, East-1, South-2, West-3








        public WorldTile(String locationID, Coords coordinates)
        {
            this.locationID = locationID;
            this.coordinates = coordinates;

        }

        public String getTemperatureType()
        {
            String temperature_string = this.climateType.Split("-")[0];
            return temperature_string;
        }
        public String getHydrationType()
        {
            String hydration_string = this.climateType.Split("-")[1];
            return hydration_string;
        }

        public void SetBorder(int direction, bool value)  {
            if (direction >= 0 && direction < 4)
            {
                riverBorders[direction] = value;
            }
            else
            {
                throw new ArgumentException("Invalid direction. Valid values are 0 (North), 1 (East), 2 (South), or 3 (West).");
            }
        }


        public String toString1()
        {
            String toString1 = "TILE: " + coordinates.ToString() + "|| LANDTYPE:" + this.landType + " TERRAINTYPE:" + this.terrainType + " CLIMATETYPE:" + this.climateType;

            return toString1;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWB_ass1
{
    public static class BoundingBoxes
    {
        public static List<BoundingBox> mapBoxes = new List<BoundingBox>();
        public static List<BoundingBox> playerBoxes = new List<BoundingBox>();
        public static List<BoundingBox> enemyBoxes = new List<BoundingBox>();

        public static List<String> boxNames = new List<String>();

    }
}

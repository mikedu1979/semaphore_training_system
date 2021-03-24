using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semaphore_training_system
{
    static class poseConfiguration
    {
        static public int[] leftHandAngle = { -90,-90,-90,-90,45,0,-45,-135,-135,0,90,45,0,-45,180,90,45,0,-45,90,45,45,-45,45,45,0,0,90,};

        static public int[] rightHandAngle = { -45,0,45,90,-90,-90,-90,-90,45,90,-45,-45,-45,-45,45,0,0,0,0,45,45,90,90,180,-135,45,-135,90,};

        static public char[] character = { 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','#','V','W','X','Y','Z','|'};
    }
}

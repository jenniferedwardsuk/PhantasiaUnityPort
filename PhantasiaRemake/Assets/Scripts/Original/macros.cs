using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using phantasiaclasses;

public class macros //: MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //#define 
    static int randoffset = 0;
    internal static double RND()
    {
        //#define RND()		((double)Do_random() / 2147483647.0)        //Do_random is from miscclass : return a random floating point number from 0.0 < 1.0

        //double randNum = 0;
        //double range = 2147483647.0 - 1;
        //randNum = rand.NextDouble() * range + 1; //NextDouble is between 0 and 1
        //return randNum;
        randoffset++;
        System.Random rand = new System.Random(CFUNCTIONS.GetMillisecs(DateTime.Now) + randoffset); //todo: random enough?
        //rand.NextDouble(); //increase randomness?

        double divisor = 2147483647.0;
        double randNum = rand.NextDouble();// / divisor; //commented divisor; NextDouble is between 0 and 1

        //Debug.LogError("Rand debug: " + randNum);
        return randNum;
    }
    //{ return (double)UnityEngine.Random.Range(1f, 2147483647.0f) / 2147483647.0; } //not thread-compatible

    //#define 
    internal static double ROLL(double BASE, double INTERVAL)
    {
        return (double)Mathf.Floor((float)(BASE + (INTERVAL * macros.RND())));
    }
    internal static float ROLL(float BASE, float INTERVAL)
    {
        return (float)Mathf.Floor((float)(BASE + (INTERVAL * macros.RND())));
    }
    internal static int ROLL(int BASE, int INTERVAL)
    {
        return (int)Mathf.Floor((float)(BASE + (INTERVAL * macros.RND())));
    }
    //#define 
    internal static int SGN<T>(T X) 
        where T : IComparable
    {
        return X.CompareTo(0) < 0 ? -1 : 1;
    }
    //    /*
    //    #define CIRCLE(X, Y)    cfunctions.floor(miscclass.Do_distance(X, 0.0, Y, 0.0) / phantdefs.D_CIRCLE + 1)
    //    */
    //#define 
    //internal static int ANY<T>(T X) where T : IComparable { return X.CompareTo(0) > 0 ? 1 : 0; } //fails with non-int16. only called with short anyway
    internal static int ANY(short X) { return X.CompareTo(0) > 0 ? 1 : 0; }
    //#define 
    internal static bool CRACKS(client_t c) { return ((c.player.circle > 25) && (c.player.circle < 30)); }
    //#define 
    internal static T MAX<T>(T A, T B) where T : IComparable { return A.CompareTo(B) > 0 ? A : B; }
    //#define 
    internal static T MIN<T>(T A, T B) where T : IComparable { return A.CompareTo(B) < 0 ? A : B; }
    //#define 
    internal static double CALCLEVEL(double XP) { return Mathf.Floor(Mathf.Sqrt((float)(XP / 1800.0))); }
    internal static float CALCLEVEL(float XP) { return Mathf.Floor(Mathf.Sqrt((float)(XP / 1800.0))); }
    internal static int CALCLEVEL(int XP) { return (int)Mathf.Floor(Mathf.Sqrt(((float)XP / 1800.0f))); }

}

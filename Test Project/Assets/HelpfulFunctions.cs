using UnityEngine;

// Some functions that could help me debug!

class HelpfulFunctions
{
    public static void printStringArray(string[] array)
    {
        if (array.Length > 0)
        {
            string stringOfArray = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                stringOfArray += ", " + array[i];
            }
            Debug.Log(stringOfArray);
        }
    }


    public static string arrayToString(string[] array)
    {
        if (array.Length > 0)
        {
            string stringOfArray = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                stringOfArray += ", " + array[i];
            }
            return stringOfArray;
        }
        else
        {
            return "[]";
        }
    }
}



/*
[Goto Scene A1]

[Scene A1]
Mom:All right, Elaine! Time for another year at school!
Elaine:You sound so excited mom. I'm pretty sure I wasn't bothering you that much over the summer!
Mom:Ahaha, of course you weren't, dear! It's just exciting to see you start your senior year of high school.
Mom:It felt like just yesterday when we were first enrolling you into this school... but enough with me getting all nostalgic on you. Go on and have a great first day of school!
Elaine:R-right! Thanks mom!
ChoiceTrigger:Mom:So are you ready to go?
[Goto Scene A3]:Yep!
[Goto Scene A4]:Sure...

[Scene A3]
Elaine:Yep!
Mom:Okay! Have a good day, honey!
[Goto Scene A5]

[Scene A4]
Elaine:Sure...
Mom:Don't sound so down! Have a good day, honey!
[Goto Scene A5]

[Scene A5]
Elaine:Looks like this is the end of the scene...
Elaine:Better head to school, then!
ChoiceTrigger:Mom:I love you!
[Goto Scene A6]:You, too!
[Goto Scene A7]:...

[Scene A6]
Elaine:You, too!
[Goto Scene A8]

[Scene A7]
Elaine:...
[Goto Scene A8]

[Scene A8]
Elaine:Bye!



*/

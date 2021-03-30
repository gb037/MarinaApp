using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace MarinaApp
{
    //This class controls the user interface
    class MarinaAdmin
    {
        static void Main(string[] args)
        {
            //Declaring variables
            string errMessage = string.Empty;
            var myMarina = new Marina();
            string menuChoice;
            bool boatRemoved = false;
            bool boatLengthOK = false;
            bool boatDraftOK = false;
            bool totalBoatLengthOK = false;
            string priceResponse;
            bool priceApprovalOK = false;

            Console.Title = "Marina App";

            //Looks for file
            myMarina.OpenSavedFile();

            //Displays the Menu
            Console.Write("Enter New Boat (1), Delete Boat (2), View Boats (3), or Quit (4): ");
            menuChoice = Console.ReadLine();

            while (menuChoice != "4")
            {
                //New Boat option selected
                if (menuChoice == "1")
                {
                    Console.WriteLine("\n------ New Boat ------");
                    int bLength = CaptureBoatLength();
                    boatLengthOK = myMarina.CheckBoatLength(bLength);
                    if (!boatLengthOK) // failed check
                    {
                        Console.WriteLine("\n========== Failed. Reason: The length of the boat is longer than allowed ==========");
                    }
                    else //proceed with rest of Admin process
                    {
                        Console.WriteLine("\n===== Boat length OK =====");
                        int bDraft = CaptureBoatDraft();
                        boatDraftOK = myMarina.CheckBoatDraft(bDraft);
                        if (!boatDraftOK) // failed check
                        {
                            Console.WriteLine("\n========== Failed. Reason: The draft of the boat is greater than allowed ==========");
                        }
                        else //proceed with rest of Admin process
                        {
                            Console.WriteLine("\n===== Boat draft OK =====");

                            totalBoatLengthOK = myMarina.CheckTotalBoatLength(bLength);
                            if (!totalBoatLengthOK) // failed check
                            {
                                Console.WriteLine("\n========== Failed. Reason: No more room ==========");
                            }
                            else //proceed with rest of Admin process
                            {
                                Console.WriteLine("\n===== Marina space OK =====");
                                int months = CaptureBoatMonths();
                                int price = myMarina.CostOfMarinaStay(bLength, months);
                                priceResponse = CapturePriceApproval(price);
                                priceApprovalOK = myMarina.ProcessOfferResponse(priceResponse);
                                if (!priceApprovalOK) // user rejected offer
                                {
                                    Console.WriteLine("\n========== Transaction cancelled. Reason: Offer rejected ==========");
                                }
                                else //proceed with rest of Admin process
                                {
                                    Console.WriteLine("\n===== Offer accepted =====");

                                    string bName = CaptureBoatName();
                                    string bOwner = CaptureBoatOwner();
                                    int boatTypeChoice = CaptureBoatType();
                                    string bType = myMarina.ConvBTypeInput(boatTypeChoice);
                                    Console.WriteLine(bType);
                                    myMarina.AddToBoatList(bLength, bDraft, bName, bOwner, bType);
                                    Console.WriteLine("\n========== " + bName + " added ==========");
                                }
                            }
                        }
                    }
                }
                //Delete Boat option selected
                else if (menuChoice == "2")
                {
                    Console.WriteLine("\n------ Delete Boat ------\n");
                    string displayString = myMarina.FormatOutput();
                    Console.WriteLine(displayString);
                    int boatNumber = CaptureBoatNumber();
                    boatRemoved = myMarina.RemoveByBoatNumber(boatNumber, ref errMessage);
                    if (!boatRemoved) //failed checks
                    {
                        Console.WriteLine("\n========== Failed. Reason: " + errMessage + " ==========");
                    }
                    else
                    {
                        Console.WriteLine("\n========== Boat deleted ==========");
                    }
                }
                //View Boats option selected
                else if (menuChoice == "3")
                {
                    Console.WriteLine("\n------ View Boats ------\n");
                    string displayString = myMarina.FormatOutput();
                    Console.WriteLine(displayString);
                    int marinaSpace = myMarina.MarinaSpace();
                    Console.WriteLine("\n" + marinaSpace + "m available in Marina");
                }
                Console.Write("\nEnter New Boat (1), Delete Boat (2), View Boats (3), or Quit (4): ");
                menuChoice = Console.ReadLine();
            }
            //Writes file
            myMarina.SaveBoatListToFile();
        }
        
        //This method gets user input for boat length
        static int CaptureBoatLength()
        {
            Console.Write("\nEnter boat length in metres: ");
            string bLengthString = Console.ReadLine();
            double bLengthDouble = Math.Ceiling(double.Parse(bLengthString));
            int bLength = (int)bLengthDouble;
            return bLength;
        }
        //This method gets user input for boat draft
        static int CaptureBoatDraft()
        {
            Console.Write("\nEnter boat draft in metres: ");
            string bDraftString = Console.ReadLine();
            double bDraftDouble = Math.Ceiling(double.Parse(bDraftString));
            int bDraft = (int)bDraftDouble;
            return bDraft;
        }
        //This method gets user input for duration of stay
        static int CaptureBoatMonths()
        {
            Console.Write("\nEnter the duration of stay in months: ");
            string monthsString = Console.ReadLine();
            int months = int.Parse(monthsString);
            return months;
        }
        //This method displays the price of berth and a request to proceed, and gets user input as approval
        static string CapturePriceApproval(int offer)
        {
            Console.Write("\nPrice of berth is £" + offer + ". Proceed? (y/n) ");
            string offerResponse = Console.ReadLine();
            return offerResponse;
        }
        //This method gets user input for boat name
        static string CaptureBoatName()
        {
            Console.Write("\nEnter boat name: ");
            string bName = Console.ReadLine();
            return bName;
        }
        //This method gets user input for boat owner
        static string CaptureBoatOwner()
        {
            Console.Write("\nEnter boat owner: ");
            string bOwner = Console.ReadLine();
            return bOwner;
        }
        //This method gets user input for which boat to delete
        static int CaptureBoatNumber()
        {
            Console.Write("Enter boat number to delete: ");
            string boatNumberString = Console.ReadLine();
            int boatNumber = int.Parse(boatNumberString);
            return boatNumber;
        }
        //This method gets user input for boat type
        static int CaptureBoatType()
        {
            Console.Write("\nEnter boat type: Narrow (1), Sailing (2), or Motor (3): ");
            string bTypeInputString = Console.ReadLine();
            int bTypeInput = int.Parse(bTypeInputString);
            return bTypeInput;
        }
    }

    //This class does the logic
    public class Marina
    {
        //Creates a new boat list
        List<Boat> boatList = new List<Boat>();
        public Marina() //Constructor
        {

        }
        //This method creates a new boat object
        public void AddToBoatList(int bLength, int bDraft, string bName, string bOwner, string bType)
        {
            boatList.Add(new Boat(bLength, bDraft, bName, bOwner, bType));
        }

        //This method formats each list item for display (Sweet, 2015)
        public string FormatOutput()
        {

            string tempBoat = string.Format("{0,-5}", "#") + string.Format("{0,-40}", "Boat Owner") + string.Format("{0,-40}", "Boat Name") + string.Format("{0,-15}", "Boat Length") + string.Format("{0,-10}", "Boat Type") + "\n\n";
            int i = 1;
            foreach (var boatItem in boatList)
            {

                tempBoat += string.Format("{0,-5}", i++) + string.Format("{0,-40}", boatItem.boatOwner) + string.Format("{0,-40}", boatItem.boatName.ToString()) + string.Format("{0,-15}", boatItem.boatLength.ToString() + "m") + string.Format("{0,-10}", boatItem.boatType.ToString());
                tempBoat += "\n";

            }
            return tempBoat;
        }
        //This method checks the boat length is no more than 15m
        public bool CheckBoatLength(int bLength)
        {
            if (bLength > 15)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //This method checks the boat draft is no more than 5m
        public bool CheckBoatDraft(int bDraft)
        {
            if (bDraft > 5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //This method checks all boat lengths plus the new boat length is no more than 150m
        public bool CheckTotalBoatLength(int lengthOfNewBoat)
        {
            int totalLength = 0;
            if (boatList.Count > 0)
            {
                foreach (var boatItem in boatList)
                {
                    totalLength += boatItem.boatLength;
                }
            }
            if ((totalLength + lengthOfNewBoat) > 150)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //This method calculates the amount of free space in the Marina
        public int MarinaSpace()
        {
            int totalLength = 0;
            foreach (var boatItem in boatList)
            {
                totalLength += boatItem.boatLength;
            }
            int marinaSpace = 150 - totalLength;
            return marinaSpace;
        }

        //This method calculates the price of berth
        public int CostOfMarinaStay(int bLength, int months)
        {
            int rate = 10;
            int totalPrice = rate * bLength * months;
            return totalPrice;
        }
        //This method converts the user's response to the offer from a string to a bool
        public bool ProcessOfferResponse(string userResponse)
        {
            if (userResponse == "n")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //This method converts the boat number that the user input to an index number, and sends it to CheckBoatNumber() method.
        //If it returns positive, the boat item with that index number is removed from the list (de Kort, 2012).
        public bool RemoveByBoatNumber(int boatNumber, ref string errMessage)
        {
            int boatIndex = boatNumber - 1;
            bool allowedToRemove = true;
            allowedToRemove = CheckBoatNumber(boatNumber);
            if (!allowedToRemove)
            {
                errMessage = "There's no boat with that number";
            }
            if (allowedToRemove)
            {
                boatList.RemoveAt(boatIndex);
            }

            return allowedToRemove;
        }
        //This method checks that there is a boat under the number that the user input
        public bool CheckBoatNumber(int boatNumber)
        {
            if (boatNumber > boatList.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //This method opens an XML file and converts each line into an item in the <list> (Grove, 2013)
        //If there is no XML file, it presents a greeting
        public void OpenSavedFile()
        {
            List<String> xmlList = new List<String>();
            List<String> temp = new List<String>();
            try
            {
                var serializer = new XmlSerializer(typeof(List<string>));
                using (var stream = File.OpenRead("listofBoats.xml"))
                {
                    var other = (List<string>)(serializer.Deserialize(stream));
                    xmlList.Clear();
                    xmlList.AddRange(other);
                }

                foreach (var item in xmlList)
                {
                    temp = item.Split(',').ToList<String>();
                    boatList.Add(new Boat(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), temp[2].ToString(), temp[3].ToString(), temp[4].ToString()));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Welcome to Marina App.\n");
            }
        }
        //This method save a <list> to an XML file (Grove, 2013)
        public void SaveBoatListToFile()
        {
            List<String> xmlList = new List<String>();
            if (boatList.Count > 0)
            {
                string temp = string.Empty;
                foreach (var boatItem in boatList)
                {
                    temp = boatItem.boatLength.ToString() + "," + boatItem.boatDraft.ToString() + "," + boatItem.boatName + "," + boatItem.boatOwner + "," + boatItem.boatType;
                    xmlList.Add(temp);
                }
            }
            var serializer = new XmlSerializer(typeof(List<String>));
            using (var stream = File.OpenWrite("listofBoats.xml"))
            {
                serializer.Serialize(stream, xmlList);
            }
        }
        //This method converts the user's response to boat type from an int to a string
        public string ConvBTypeInput(int boatTResponse)
        {
            if (boatTResponse == 1)
            {
                return "Narrow";
            }
            else if (boatTResponse == 2)
            {
                return "Sailing";
            }
            else
            {
                return "Motor";
            }
        }
    }

    //This class encapsulates the idea of a boat
    public class Boat
    {
        //Declaring the boat properties (Wenzel, et al., 2017)
        private int btLen;
        private int btDft;
        private string btNam;
        private string btOnr;
        private string btTyp;

        //This method controls the accessibility of the boat properties
        public Boat(int bLength, int bDraft, string bName, string bOwner, string bType)
        {
            boatLength = bLength;
            boatDraft = bDraft;
            boatName = bName;
            boatOwner = bOwner;
            boatType = bType;
        }
        //This method controls the accessibility of the boat length property
        public int boatLength
        {
            get
            {
                return btLen;
            }

            set
            {
                btLen = value;
            }
        }
        //This method controls the accessibility of the boat draft property
        public int boatDraft
        {
            get
            {
                return btDft;
            }

            set
            {
                btDft = value;
            }
        }
        //This method controls the accessibility of the boat owner property
        public string boatOwner
        {
            get
            {
                return btOnr;
            }

            set
            {
                btOnr = value;
            }
        }
        //This method controls the accessibility of the boat name property
        public string boatName
        {
            get
            {
                return btNam;
            }

            set
            {
                btNam = value;
            }
        }
        //This method controls the accessibility of the boat type property
        public string boatType
        {
            get
            {
                return btTyp;
            }

            set
            {
                btTyp = value;
            }
        }
    }
}
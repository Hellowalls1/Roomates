﻿using System;
using System.Collections.Generic;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        /// <summary>
        ///  This is the address of the database.
        ///  We define it here as a constant since it will never change.
        /// </summary>
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true";

        static void Main(string[] args)
        {

            //roomRepo is where you store things as RoomInsert and get all method
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);

            Console.WriteLine("Getting All Rooms:");
            Console.WriteLine();



            //program will make a connection to the database, run the query, bring data back, turn into Room objects, and then
            //return it to us so we can iterate over it and return the information on lines 28-31

            List<Room> allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }


            Room bathroom = new Room
            {
                Name = "Bathroom",
                MaxOccupancy = 3
            };

            Room lavatory = new Room
            {
                Name = "Lavatory",
                MaxOccupancy = 12
            };

            //passing bathroom into rooms repository opjecct
            //will save it to the database
            //we set the id inside the insert method

            roomRepo.Insert(bathroom);
          //  roomRepo.Insert(lavatory);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Room with id {bathroom.Id}");

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Updating Room with id {bathroom.Id}");

            Room updatedBathroom = new Room
            {
                Name = "Washroom",
                MaxOccupancy = 15,
                Id = bathroom.Id
               
            };

            roomRepo.Update(updatedBathroom);

           roomRepo.Delete(8);
            roomRepo.Delete(10);
            roomRepo.Delete(11);
            roomRepo.Delete(12);
            roomRepo.Delete(16);
            roomRepo.Delete(17);
            roomRepo.Delete(18);
            roomRepo.Delete(19);
            roomRepo.Delete(20);

            allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }
        }
    }
}
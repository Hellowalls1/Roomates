﻿using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;

namespace Roommates.Repositories
{

    public class RoommateRepository : BaseRepository
    {



        public RoommateRepository(string connectionString) : base(connectionString) { }


        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        /// 
        //creating a public method that returns the rooms goes to the database and returns all the rooms
        public List<Roommate> GetAll()

        {   //creating a connection to connect the server to database
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            //Connection lives in our base repository we've inherited that
            //using block gives us the ability to automatically close and destroy the connection when we are done with it
            //create the connection and it is no longer available  at the end of the using block at the ending bracket in the 80s line

            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                //opening up the pathway or tunnel/connection
                conn.Open();

                // 3. saying hey do this thing for me/executing command
                //when executed it returns us a reader object (thing we do to get the data from the results)
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    //Sending the actual text that is the SQL text getting sent to the database
                    //this is selecting all the data from the database and needs to line up with object instantiation below
                    cmd.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate, RoomId FROM Roommate";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    //Take the sql that is above and send it to the database/ then the database executes that query for us

                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the roommates we retrieve from the databae
                    List<Roommate> roommates = new List<Roommate>();

                    // Read() will return true if there's more data to read
                    //will continue to run until we run out of data
                    //the purpose is to read the data from our result set and create a room
                    while (reader.Read())
                    {

                        //Ordinal is the column. Have to get the number of the column
                        //The get the data
                        //reader.GetOrdinal("ID") is giving the value of that column
                        //getting all of the values below based off the id

                        int idColumnPosition = reader.GetOrdinal("Id");


                        int idValue = reader.GetInt32(idColumnPosition);

                        int FirstnameColumnPosition = reader.GetOrdinal("FirstName");
                        string FirstnameValue = reader.GetString(FirstnameColumnPosition);

                        int LastnameColumnPosition = reader.GetOrdinal("LastName");
                        string LastnameValue = reader.GetString(LastnameColumnPosition);


                        int RentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                        int RentPortionValue = reader.GetInt32(RentPortionColumnPosition);

                        int MovedInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                        DateTime MovedInDateValue = reader.GetDateTime(MovedInDateColumnPosition);


                        int RoomIdColumnPosition = reader.GetOrdinal("RoomId");
                        int RoomIdValue = reader.GetInt32(RoomIdColumnPosition);

                        // Now let's create a new room object using the data from the database.
                        //create a room object asbout that room object
                        Roommate newRoommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = FirstnameValue,
                            LastName = LastnameValue,
                            RentPortion = RentPortionValue,
                            MoveInDate = MovedInDateValue,
                            Room = null
                        };

                        // ...and add that newRoommate object to our "roomates" list.
                        roommates.Add(newRoommate);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of roommates who whomever called this method.

                    return roommates;
                }


            }
        }

        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        /// 

        //Whoever calls this insert method is required to provide a Room object
        //goin to take the values from the Room object and insert them into the database

        /// <summary>
        ///Returns a single room with the given id.
        /// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstName, LastName, RentPortion, MoveInDate, RoomId FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MoveInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = null

                        };

                    }

                    reader.Close();

                    return roommate;
                }
            }
        }


        public List<Roommate> GetAllWithRoom(int roomId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @$"SELECT 
                                        roommate.FirstName,
                                        roommate.LastName
                                        room.Name
                                        From Roommate roommate
                                        Join Room room ON roommate.RoomId = room.Id
                                        WHERE roommate.roomId = {roomId}";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommateRoom = new List<Roommate>();
                    // If we only expect a single row back from the database, we don't need a while loop.
                    while (reader.Read())
                    {
                        Roommate eachRoommate = new Roommate
                        {
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Room = new Room()

                            {
                                Id = roomId,
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };

                        roommateRoom.Add(eachRoommate);
                    }

                reader.Close();
                return roommateRoom;
                };

             
        }
    }

}
}

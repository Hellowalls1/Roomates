using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoomRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRespository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        /// Everytime we want to talk to the database we need to make a connection
        /// constructor method that takes a constructor string
        

        public RoomRepository(string connectionString) : base(connectionString) { }


        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        /// 
        //creating a public method that returns the rooms goes to the database and returns all the rooms
        public List<Room> GetAll()

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

                // We must "use" commands too.
                //we have to send a message across the connection
                // 3. saying hey do this thing for me/executing command
                //when executed it returns us a reader object (thing we do to get the data from the results)
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    //Sending the actual text that is the SQL text getting sent to the database
                    // 4. tell the database what command we want to run
                    //this creates a result set in sequal
                    cmd.CommandText = "SELECT Id, Name, MaxOccupancy FROM Room";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    //Take the sql that is above and send it to the database/ then the database executes that query for us
                    //Execute means execute command "reader" talks about the thing it returns
                    //reader is the way you can read the result/way you can access the data from the "cmd.ExecuteReader()"
                    //as long as there is data it will return true. When it runs out it will return false
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    //going to use all the data to create the elements of this list
                    List<Room> rooms = new List<Room>();

                    // Read() will return true if there's more data to read
                    //will continue to run until we run out of data
                    //the purpose is to read the data from our result set and create a room
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        //Ordinal is the column. Have to get the number of the column
                        //The get the data
                        //reader.GetOrdinal("ID") is giving the value of that column
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        //get the Occupancy for every row

                        int maxOccupancyColunPosition = reader.GetOrdinal("MaxOccupancy");
                        int maxOccupancy = reader.GetInt32(maxOccupancyColunPosition);

                        // Now let's create a new room object using the data from the database.
                        //create a room object asbout that room object
                        Room room = new Room
                        {
                            Id = idValue,
                            Name = nameValue,
                            MaxOccupancy = maxOccupancy,
                        };

                        // ...and add that room object to our list.
                        rooms.Add(room);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.

                    return rooms;
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
        public void Insert(Room room)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    //telling the databse to insert a Room into the room database


                    //creating the rooms into the table
                    //id is automatically generated
                    //insert values into these two colums Name and Max
                    //Output line is getting the id
                    //Setting the values
                    //this is saving a room record or id/value pairs into the table
                    cmd.CommandText = @"INSERT INTO Room (Name, MaxOccupancy) 
                                              OUTPUT INSERTED.Id 
                                              VALUES (@name, @maxOccupancy)"; //


                    //referencing the Name of the room properrty (room.Name)
                    //thats the value that we want to insert
                    //the sql argument has to match the same name in our insert statement
                    //@name refers to name of the room

                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);

                    //cmd is our command object and Execute is the connection the database

                    //Scalar is a term that means an individual/single value
                    //when we call ExecuteScalar we get an integer back which is the id that was just inserted
                    int id = (int)cmd.ExecuteScalar();

                    room.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        /// <summary>
        ///  Updates the room
        /// </summary>
        public void Update(Room room)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Room
                                    SET Name = @name,
                                        MaxOccupancy = @maxOccupancy
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);
                    cmd.Parameters.AddWithValue("@id", room.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Delete the room with the given id
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Room WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

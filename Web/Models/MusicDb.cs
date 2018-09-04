﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using Microsoft.Win32;

namespace Music.Models
{
    public class MusicDb 
    {
        

        /// <summary>
        /// Retrive a list of all Songs from the database
        /// </summary>
        /// <returns></returns>
        public List<Album> GetAlbums()
        {
            // Connect to MySQL and load all the photos
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");

            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";";
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            // Create Command
            string SQL = "";
            SQL = "select A.ALBUM_ID, A.ALBUM_NAME " +
                "FROM music.ALBUM A " +
                "where A.active = 'Y' ";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            List<Album> _albums = new List<Album>();
            while (dataReader.Read())
            {
                Album item = new Album()
                {
                    Id = dataReader["album_id"].ToString(),
                    AlbumName = dataReader["album_name"].ToString(),
                };
                _albums.Add(item);
            }

            //close Data Reader
            dataReader.Close();
            conn.Close();

            return _albums;
        }

        /// <summary>
        /// Retrive a list of all Songs from the database
        /// </summary>
        /// <returns></returns>
        public List<Song> GetSongs(int AlbumId)
        {
            // Connect to MySQL and load all the photos
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");

            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";";
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            // Create Command
            string SQL = "";
                SQL = "select S.SONG_ID, S.SONG_NAME, A.ALBUM_NAME, S.FILENAME, ps.playlist_song_id " +
                    "FROM music.song s " +
                    "LEFT JOIN music.album a ON a.album_id = s.album_id " +
                    "LEFT OUTER JOIN music.playlist_song ps ON ps.song_id = s.song_id " +
                    "where s.active = 'Y' " +
            //        "and A.Album_name = 'Believe' " +
            //        "and s.Album_id = A.ALBUM_ID " +
                    "and s.Album_id = " + AlbumId + " " +
                    "order by A.album_name, s.song_name ";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            List<Song> _songs = new List<Song>();
            while (dataReader.Read())
            {
                Song item = new Song()
                {
                    Id = dataReader["song_id"].ToString(),
                    Title = dataReader["song_name"].ToString(),
                    Album = dataReader["album_name"].ToString(),
                    Location = dataReader["filename"].ToString(),
                    PlaylistSongID = dataReader["playlist_song_id"].ToString(),
                };
                _songs.Add(item);
            }

            //close Data Reader
            dataReader.Close();
            conn.Close();

            return _songs;
        }



        /// <summary>
        /// Àdd a Song to the Database
        /// </summary>
        /// <returns></returns>
        public bool AddSongToPlaylist(string PlayListID, string SongID)
        {
            try { 
                // Connect to MySQL and load all the photos
                MySql.Data.MySqlClient.MySqlConnection conn;
                string myConnectionString;

                // Get the connection password
                string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");

                myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";";
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();

                // Create Command
                string SQL = "";
                SQL = "insert into music.playlist_song (created_date, created_by_id, playlist_id, song_id, active) values (now(), 'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3',  '" + PlayListID + "', '" + SongID + "','Y') ";

                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                //Create a data reader and Execute the command
                cmd.ExecuteNonQuery();

                // Set flag to regenerate playlist
                SQL = "update music.setting set value='Y', created_date = now(), created_by_id =  'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3'  where setting = 'Reset Google Playlist' ";
                cmd = new MySqlCommand(SQL, conn);
                //Create a data reader and Execute the command
                cmd.ExecuteNonQuery();

                conn.Close();

                return true;
            } catch
            {
                return false;
            }
        }

       

        //////////////////////////////////////////////////////////
        // Playlist

        /// <summary>
        /// Retrive a list of all albums from the database
        /// </summary>
        /// <param name="Limit">No. of Blog Entries to return</param>
        /// <returns></returns>
        public List<Playlist> GetPlaylists(int Limit, string UserID)
        {
            // Connect to MySQL and load all the photos
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");

            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";";
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            // Create the SQL command - This is a union statement because if the user is an Administrator
            //                          we do not want to restrict the blogs returned
            string SQL = "";
            SQL = "select playlist_id, playlist_name, DATE_FORMAT(GREATEST(p.CREATED_DATE, ifnull(p.UPDATED_DATE, p.CREATED_DATE)), '%d-%M-%Y') as modified_date, u.username from music.playlist p, user.aspnetusers u where p.active='Y' and u.id = p.owner_id order by playlist_name";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            List<Playlist> _playlists = new List<Playlist>();
            while (dataReader.Read())
            {
                Playlist item = new Playlist()
                {
                    Id = dataReader["playlist_id"].ToString(),
                    PlaylistName = dataReader["playlist_name"].ToString(),
                    DateModified = dataReader["modified_date"].ToString(),
                    Owner = dataReader["username"].ToString(),
                };
                _playlists.Add(item);
            }

            //close Data Reader
            dataReader.Close();
            conn.Close();

            return _playlists;
        }




        /// <summary>
        /// Retrive a single blog entry from the database
        /// </summary>
        /// <param name="BlogId">the blog Id to load</param>
        /// <returns></returns>
        public Playlist GetPlaylist(int PlaylistId, string UserID)
        {
            // Connect to MySQL and load all the photos
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");

            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";";
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            // Create Command
            string SQL = @"select p.playlist_id, p.playlist_name, a.username, DATE_FORMAT(GREATEST(p.CREATED_DATE, ifnull(p.UPDATED_DATE, p.CREATED_DATE)), '%d-%M-%Y') as modified_date 
            from music.playlist p, user.aspnetusers a where p.owner_id = a.id and p.playlist_id = " + PlaylistId;

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list            
            Playlist _playlist = new Playlist();
            if (dataReader.Read())
            {
                _playlist.Id = dataReader["playlist_id"].ToString();
                _playlist.PlaylistName = dataReader["playlist_name"].ToString();
                _playlist.Owner = dataReader["username"].ToString();
                _playlist.DateModified = dataReader["modified_date"].ToString();
            }

            //close Data Reader
            dataReader.Close();
            conn.Close();

            return _playlist;
        }




        /// <summary>
        /// Insert or Update a playlist entry
        /// </summary>
        /// <param name="blog">The Blog model</param>
        /// <returns></returns>
        public string SavePlaylistEntry(Playlist playlist)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");
            string myConnectionString;
            string Id = "";
            string SQL;
            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";default command timeout=0";
            conn.ConnectionString = myConnectionString;
            conn.Open();


            if (playlist.Id == "0")
            {
                // Insert
                SQL = "insert into music.playlist(created_date, created_by_id, owner_id, playlist_name,active) values(now(), 'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3', 'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3', '" + playlist.PlaylistName + "', 'Y');";
                MySqlCommand cmdInt = new MySqlCommand(SQL, conn);
                cmdInt.ExecuteNonQuery();

                // Get the ID
                SQL = "select LAST_INSERT_ID() AS MYID from music.playlist;";
                cmdInt = new MySqlCommand(SQL, conn);
                MySqlDataReader dataReader = cmdInt.ExecuteReader();
                dataReader.Read();
                Id = dataReader["MYID"].ToString();
            }
            else
            {
                SQL = "update music.playlist set playlist_name = '" + playlist.PlaylistName + "', updated_by_id = 'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3', updated_date = now() where playlist_id = " + playlist.Id;
                MySqlCommand cmdInt = new MySqlCommand(SQL, conn);
                cmdInt.ExecuteNonQuery();
                Id = playlist.Id;
            }

            // Set flag to regenerate playlist
            SQL = "update music.setting set value='Y', created_date = now(), created_by_id =  'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3'  where setting = 'Reset Google Playlist' ";
            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            cmd.ExecuteNonQuery();

            conn.Close();
            return Id;
        }

        /// <summary>
        /// Delete a blog entry
        /// </summary>
        /// <param name="Id">The Blog Id</param>
        /// <returns></returns>
        public void DeletePlaylistEntry(string Id)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            // Get the connection password
            string password = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Lattuce", "MySQLPassword", "");
            string myConnectionString;
            string SQL;
            myConnectionString = "Server=lattuce-dc;Database=photos;Uid=root;Pwd=" + password + ";default command timeout=0";
            conn.ConnectionString = myConnectionString;
            conn.Open();

            SQL = "update music.playlist set active = 'N', updated_by_id = 'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3', updated_date = now() where playlist_id = " + Id;
            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();

            // Set flag to regenerate playlist
            SQL = "update music.setting set value='Y', created_date = now(), created_by_id =  'feb66d43-7615-4dbe-93f1-73cc4b4bf2a3'  where setting = 'Reset Google Playlist' ";
            cmd = new MySqlCommand(SQL, conn);
            //Create a data reader and Execute the command
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        
    }
}
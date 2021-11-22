package edu.ucalgary.ensf409;
import java.sql.*;

/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 2.3
 */

public class DatabaseReader {
    private Connection connection;
    private PreparedStatement classStmt;
    private final String DBURL;
    private final String USERNAME;
    private final String PASSWORD;

    /**
     * Intitalizes the needed info to connect to the database
     * @param DBURL
     * @param USERNAME
     * @param PASSWORD
     */
    public DatabaseReader(String DBURL, String USERNAME, String PASSWORD) {
        this.DBURL = DBURL;
        this.USERNAME = USERNAME;
        this.PASSWORD = PASSWORD;
        this.initializeConnection();
    }

    public PreparedStatement getClassStmt() {
        return this.classStmt;
    }

    public void setClassStmt(PreparedStatement classStmt) {
        this.classStmt = classStmt;
    }

    public String getDBURL() {
        return this.DBURL;
    }

    public String getUSERNAME() {
        return this.USERNAME;
    }

    public String getPASSWORD() {
        return this.PASSWORD;
    }

    /**
     * Initializes the connection to the database using the 
     * DBURL, USERNAME, and PASSWORD
     */
    public void initializeConnection() {
        try {
            connection = DriverManager.getConnection(DBURL, USERNAME, PASSWORD);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    /**
     * Closes the connection and statement held in 
     * class variables
     */
    public void close() {
        try {
            this.connection.close();
        } catch (SQLException e) {
            System.out.println("Wouldn't close");
        }
    }

    public boolean requestedNumberIsPossible(int num, String table, String type) { 
        try {
            if (checkForItem(table, type) < num) {
                return false;
            } 

            String query = "SELECT * FROM " + table + " WHERE Type = ?";
            PreparedStatement myStmt = connection.prepareStatement(query);
            myStmt.setString(1, type);
            ResultSet result = myStmt.executeQuery();

            ResultSetMetaData resultMD = result.getMetaData();
            int columnCount = resultMD.getColumnCount();
            int[] numOfYCount = new int[columnCount - 4];

            while (result.next()) {
                for (int i = 3; i <= columnCount - 2; i++) {
                    if (result.getString(i).equals("Y")) {
                        numOfYCount[i - 3]++;
                    }
                }
            }

            for (int i = 0; i < numOfYCount.length; i++) {
                if (numOfYCount[i] < num) {
                    return false;
                }
            }

            return true;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    /**
     * A function that returns all the results that contain the type required 
     * in the table of the database.
     * @param table Table to search
     * @param type Type of item to look for
     * @return The ResultSet object containing all the results
     */
    public ResultSet getItems(String table, String type) {
        try {
            String query = "SELECT * FROM " + table + " WHERE Type = ?";
            classStmt = connection.prepareStatement(query);
            classStmt.setString(1, type);
            ResultSet result = classStmt.executeQuery();

            // result's connection and statement will be stored in the class variables
            return result;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

    /**
     * Searches for items in the table specified that match the type specified and 
     * returns the number of items that match.
     * @param table The table to search in
     * @param type the type of item in the table
     * @return the number of items in the table
     */
    public int checkForItem(String table, String type) {
        int num = 0;
        try {
            String query = "SELECT * FROM " + table + " WHERE Type = ?";
            PreparedStatement myStmt = connection.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY);
            myStmt.setString(1, type);
            ResultSet result = myStmt.executeQuery();
            if (result.last()) {
                num = result.getRow();
            }

            myStmt.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        
        return num;
    }

    /**
     * A method to remove an item with a certain id from the specified table
     * @param id ID of the furniture item
     * @param tableName Table name of furniture item, ex: chair
     */
    public void removeItem(String id, String table) {
        try {
            String query = "DELETE FROM " + table + " WHERE ID = ?";
            PreparedStatement myStmt = connection.prepareStatement(query);
            myStmt.setString(1, id);
            myStmt.executeUpdate();

            myStmt.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    /**
     * Takes in the ID number of an item (expected to be in the table)
     * and uses it to return a String array containing the id, parts, and
     * cost of that item.
     * @param id Specified ID Number
     * @param table Specified category of the item
     * @return String[] containing the id, parts, and cost of the item specified
     */
    public String[] IDToInfo(String id, String table)
    {
       
        try {
            String query = "SELECT * FROM " + table + " WHERE ID = ?";
            PreparedStatement myStmt = connection.prepareStatement(query);
            myStmt.setString(1, id);

            ResultSet result = myStmt.executeQuery();
            ResultSetMetaData metaData = result.getMetaData();
            int columnCount = metaData.getColumnCount();

            if (result.next())
            {
                String[] singleRow = new String[columnCount - 2];
                singleRow[0] = result.getString(1);

                for (int i = 3; i <= columnCount - 1; i++)
                {
                    singleRow[i-2] = result.getString(i);
                }
                return singleRow;
            }

            myStmt.close();

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }
} 

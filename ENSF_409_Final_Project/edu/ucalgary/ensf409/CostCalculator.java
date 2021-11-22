package edu.ucalgary.ensf409;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 2.2
 */

public class CostCalculator
{
    private DatabaseReader databaseReader;

    /**
     * Default constructor of the CostCalculator class
     */
    public CostCalculator(DatabaseReader databaseReader)
    {
        this.databaseReader = databaseReader;
    }
    /**
     * 
     * @return stored databaseReader
     */
    public DatabaseReader getDatabaseReader(){
        return this.databaseReader;
    }

    /**
     * Determines the combination of items that has the lowest collective cost
     * for the order specified by the client. If there is not at least one possible
     * combination of items to make the number of items specified, the function
     * will return null.
     * @param table The chosen category in the order
     * @param type The chosen type of the category in the order
     * @param numberOfItems The chosen number of items for the order
     * @return Returns a String array of the IDs of the combination of items
     * that has the lowest collective cost.
     * If there is not a single combination possible, the function returns null
     */
    public String[] calculateBestSet(int numberOfItems, String table, String type)
    {

        String[] allIDs = getAllMatchingIDs(table, type);

        ArrayList<String[]> allIDCombos = getAllSubsetsOfList(allIDs);

        if (allIDCombos == null)
        {
            return null;
        }

        int indexOfBestSet = -1;
        double bestPrice = Double.MAX_VALUE;

        for (int i = 0; i < allIDCombos.size(); i++)
        {
            if (!IDSetCanMakeNumberOfItems(numberOfItems, allIDCombos.get(i), table, type))
            {
                continue;
            }
            double cost = costOfIDSet(allIDCombos.get(i), table);
            if (cost < bestPrice)
            {
                bestPrice = cost;
                indexOfBestSet = i;
            }
        }

        if (indexOfBestSet == -1)
        {
            return null;
        }

        return allIDCombos.get(indexOfBestSet);
    }

    /**
     * Provides an ArrayList of all possible "sublists" of
     * a given array (including a list with no elements).
     * These "sublists" are similar to subsets of a set in that they are
     * made up of the elements of the 'fullList' array given.
     * Ex: Given fullList = {"1","2","3"};
     * The function would return an ArrayList containing:
     * {},{1},{2},{2,1},{3},{3,1},{3,2},{3,2,1}
     * @param fullList Source String array from which powerset is formed
     * @return An ArrayList containing all possible "sublists" of
     * given array
     */
    public ArrayList<String[]> getAllSubsetsOfList(String[] fullList)
    {

        ArrayList<String> fullArrayList = new ArrayList<String>();

        for (int i = 0; i < fullList.length; i++)
        {
            fullArrayList.add(fullList[i]);
        }

        if (fullArrayList.isEmpty())
        {
            ArrayList<String[]> newy = new ArrayList<String[]>();
            newy.add(new String[0]);
            return newy;
        }
        else
        {
            ArrayList<String[]> finalRes = new ArrayList<String[]>();

            List<String> subList = fullArrayList.subList(1,fullArrayList.size());
            String[] listToArray = new String[subList.size()];

            for (int k = 0; k < listToArray.length; k++)
            {
                listToArray[k] = subList.get(k);
            }

            for (String[] smallList : getAllSubsetsOfList(listToArray))
            {
                finalRes.add(smallList);

                String[] temp = new String[smallList.length+1];
                
                for (int i = 0; i < smallList.length; i++)
                {
                    temp[i] = smallList[i];
                }

                temp[smallList.length] = fullArrayList.get(0);

                finalRes.add(temp);
            }

            return finalRes;
        }
    }

     /**
     * This function provides the cost of the of a given "ID Set".
     * The "ID Set" is composed of the IDs of associated items expected
     * to be found in the category of 'table'.
     * @param idSet The "IDSet" - String of IDs to associated items in category 'table'
     * @param table The category the ID's in the "ID Set" are expected to be from
     * @return The cost of the associated items in the "ID Set"
     */
    public double costOfIDSet(String[] idSet, String table)
    {

        double sum = 0;
        for (int i = 0; i < idSet.length; i++)
        {
            String[] currentIDdata = databaseReader.IDToInfo(idSet[i], table);
            sum += Double.parseDouble(currentIDdata[currentIDdata.length-1]);
        }
        return sum;
    }

     /**
     * Checks to see whether the items of associated IDs in a set of
     * IDs (an "ID Set") are able to have their parts combined to provide the
     * number of items specified. If this is true, the function returns true,
     * otherwise, the function returns false.
     * @param numberOfItems The number of items specified
     * @param idSet A String array of a set of IDs associated to items in the
     * category 'table' and of type 'type' that are going to be used to determine
     * the output.
     * @param table The category the associated items are expected to be from
     * @param type The type from the category specified that the associated
     * items are expected to be from.
     * @return The output from the function check (true or false)
     */
    public boolean IDSetCanMakeNumberOfItems(int numberOfItems, String[] idSet, String table, String type)
    {

        if (idSet == null)
        {
            return false;
        }

        try
        {
            ResultSet resultSet = databaseReader.getItems(table, type);
            ResultSetMetaData resultMD = resultSet.getMetaData();

            int columnCount = resultMD.getColumnCount();
            int[] numOfYCount = new int[columnCount - 4];

            for (int i = 0; i < idSet.length; i++)
            {
                String[] data = databaseReader.IDToInfo(idSet[i], table);
                for (int j = 1; j <= columnCount - 3; j++)
                {
                    if (data[j].equals("Y"))
                    {
                        numOfYCount[j - 1]++;
                    }
                }
            }

            for (int j = 0; j < numOfYCount.length; j++)
            {
                if (numOfYCount[j] < numberOfItems)
                {
                    return false;
                }
            }

            return true;
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }

        return false;
    }

    /**
     * Provides an array of IDs (an ID Set) of items in the category 'table'
     * that have a matching type to that specified.
     * @param table Category the items are from
     * @param type Specified furniture type
     * @return The associated array of IDs to the matching items
     */
    public String[] getAllMatchingIDs(String table, String type)
    {
        ResultSet resultSet = databaseReader.getItems(table, type);

        ArrayList<String> resultArrayList = new ArrayList<String>();
        String[] resultArray = null;

        try
        {
            while (resultSet.next())
            {
                String singleRowID = resultSet.getString(1);

                resultArrayList.add(singleRowID);
            }

            resultArray = new String[resultArrayList.size()];
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }

        if (resultArray == null) return resultArray;

        for (int i = 0; i < resultArray.length; i++)
        {
            resultArray[i] = resultArrayList.get(i);
        }

        return resultArray;
    }

}

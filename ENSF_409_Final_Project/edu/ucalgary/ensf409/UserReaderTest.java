package edu.ucalgary.ensf409;
import static org.junit.Assert.*;
import java.sql.*;
import org.junit.*;
import java.io.*;
import java.util.*;
import org.junit.contrib.java.lang.system.ExpectedSystemExit;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import jdk.jfr.Timestamp;
/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 3.2
 */
//Command line arguments needed to run the test file
//javac -cp .;lib/hamcrest-core-1.3.jar;lib/system-rules-1.19.0.jar;lib/junit-4.13.2.jar;lib/mysql-connector-java-8.0.23.jar;. edu/ucalgary/ensf409/UserReaderTest.java
//java -cp .;lib/junit-4.13.2.jar;lib/hamcrest-core-1.3.jar;lib/system-rules-1.19.0.jar;lib/mysql-connector-java-8.0.23.jar;. org.junit.runner.JUnitCore edu.ucalgary.ensf409.UserReaderTest

public class UserReaderTest{
    private final String DBURL= "jdbc:mysql://localhost/inventory";
    private final String username = "scm";
    private final String password = "ensf409";
@Test
//UserReader constructor
//checks for correct instantiation of databaseReader
//connection initialization to sql database
public void testUserReaderInstantiatesDatabaseReader(){
    UserReader tester = new UserReader(DBURL, username, password);
    String[] desired = {DBURL, username, password};
    String[] result = {tester.getDatabaseReader().getDBURL(), 
                        tester.getDatabaseReader().getUSERNAME(), 
                        tester.getDatabaseReader().getPASSWORD()};

    assertTrue("Constructor failed", Arrays.equals(desired, result));
}
@Test
//UserReader constructor
//checks for correct instantiation of CostCalculator
//connection initilization to sql database
public void testUserReaderInstantiatesCostCalculator(){
    UserReader tester = new UserReader(DBURL, username, password);
    String[] desired = {DBURL, username, password};
    String[] result = {tester.getCostCalculator().getDatabaseReader().getDBURL(), 
                        tester.getCostCalculator().getDatabaseReader().getUSERNAME(), 
                        tester.getCostCalculator().getDatabaseReader().getPASSWORD()};

    assertTrue("Constructor failed", Arrays.equals(desired, result));
}
@Test
//validateCategory
//checks with valid input with no uppercase
public void testValidateCategoryOKInput(){
    UserReader tester = new UserReader(DBURL, username, password);
    boolean result = tester.validateCategory("chair");
    assertTrue(result);
}
@Test
//validateCategory
//checks with invalid category
public void testValidateCategoryInvalidInput(){
    UserReader tester = new UserReader(DBURL, username, password);
    boolean result = tester.validateCategory("Bike");
    assertFalse("Does not check correctly for valid Category",result);
}
@Test
//validateType
//checks with invalid type and valid category
public void testValidateTypeInvalidType(){
    UserReader tester = new UserReader(DBURL, username, password);
    boolean result = tester.validateType("Bike", "chair");
    assertFalse("Does not check correctly for valid Type",result);
}
@Test
//validateType
//checks with capitalized valid type and valid category
public void testValidateTypeValidTypeCap(){
    UserReader tester = new UserReader(DBURL, username, password);
    boolean result = tester.validateType("Kneeling", "chair");
    assertTrue("validateType() does not handle capitalization",result);
}
@Test
//validateType
//checks with non-capitalized valid type and valid category
public void testValidateTypeValidTypeLow(){
    UserReader tester = new UserReader(DBURL, username, password);
    boolean result = tester.validateType("standing", "desk");
    assertTrue("validateType() does not handle all lowercase",result);
}
@Test
//requested number is possible
//valid input 1 adjustable desk
public void testrequestedNumberIsPossibleDA1(){
    DatabaseReader tester = new DatabaseReader(DBURL, username, password);
    boolean possible = tester.requestedNumberIsPossible(1,"desk","adjustable");
    assertTrue("Did not correctly identify possible options", possible);
} 
@Test
//requested number is possible
//invalid input 10 desk lamp
public void testrequestedNumberIsPossibleLD10(){
    DatabaseReader tester = new DatabaseReader(DBURL, username, password);
    boolean possible = tester.requestedNumberIsPossible(10,"lamp","desk");
    assertFalse("Incorrectly found option for an impossible request", possible);
}
@Test
//requested number is possible
//3 lamp IDs
public void testcostOfIDSet(){
    CostCalculator tester = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] set = {"L132","L342","L112"};
    double price = tester.costOfIDSet(set, "lamp");
    assertTrue("Did not correctly calculate the total cost", price ==38.00);
} 
@Test
//Calculating the best set
//input 1 item of mesh chair
public void testCalculateBestSetCM1(){
    CostCalculator tester = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] set = tester.calculateBestSet(1,"chair","mesh");
    double price = tester.costOfIDSet(set, "chair");
    assertTrue("Did not correctly calculate the lowest cost in calculateBestSet()", price==200);
}
@Test
//Calculating the best set
//input 2 item of traditional desk
//Must use parts from 1 chair to contribute to both chairs 
//to get lowest price
public void testCalculateBestSetDT2(){
    CostCalculator tester = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] set = tester.calculateBestSet(2,"desk","traditional");
    double price = tester.costOfIDSet(set, "desk");
    assertTrue("Did not correctly calculate the lowest cost in calculateBestSet()", price==200);
}
@Test
//get all matching IDs
//returns all ID numbers of a certain type and category
public void testGetAllMatchingIDs(){
    CostCalculator tester = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] result = tester.getAllMatchingIDs("chair", "mesh");
    String[] expected = {"C9890","C0942","C6748","C8138"};
    boolean found= false;
    //compare the arrays and ensure all elements of results match elements of expected
    if(result.length == expected.length){
        for(int i=0; i<result.length; i++){
            found=false;
            for(int j=0; j<expected.length; j++){
                if(result[i].equals(expected[j])){
                    found = true;
                }
            }
            if(!found){
                break;
            }
        }
    }
    assertTrue("Did not contain all the correct IDs", found);
}
@Test 
//get All sublists of list
//test with list of numbers
public void testGetAllSublistsOfList(){
    CostCalculator tester = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] set = {"Hi","Bye"};
    ArrayList<String[]> result = tester.getAllSubsetsOfList(set);
    ArrayList<String[]> expected = new ArrayList<String[]>();
    expected.add(new String[]{});
    expected.add(new String[]{"Hi"});
    expected.add(new String[]{"Bye"});
    expected.add(new String[]{"Bye","Hi"});
    boolean found = false;
    if(result.size() == expected.size()){
        for(int i=0; i<result.size(); i++){
            found=false;
            for(int j=0; j<expected.size(); j++){
                if(Arrays.equals(result.get(i),(expected.get(j)))){
                    found = true;
                }
            }
            if(!found){
                break;
            }
        }
    }
    assertTrue("Did not create the appropriate sublists",found);
}
@Test
//checks that print class creates a file
public void testPrintClassCreatesFile(){
    Printer tester = new Printer();
    DateTimeFormatter format = DateTimeFormatter.ofPattern("yyyy_MM_dd_HH_mm_ss");
    LocalDateTime time = LocalDateTime.now();
    tester.close();
    File theFile = new File("orderform" + format.format(time) + ".txt");
    assertTrue("Print class did not create a file", theFile.exists());
}
@Test
//returning item parts information given the ID and category
public void testIDToInfo(){
    DatabaseReader tester = new DatabaseReader(DBURL, username, password);

    String[] result = tester.IDToInfo("L132","lamp");
    //System.out.println(result[0]+" , "+result[1]+" , "+result[2]+" , "+result[3]);
    String[] expected = new String[]{"L132","Y","N","18"};
    assertTrue("Returned incorrect item info", Arrays.equals(result, expected));
}
@Test
//Removing items from database
public void testRemoveItem(){
    String[] set = {"F156","Small","Y","N","Y","75","003","FILING"};
    addFilingInToDatabase(set);
    DatabaseReader tester = new DatabaseReader(DBURL, username, password);
    tester.removeItem("F156","filing");
    boolean found = checkForItem("F156", "FILING");
    assertFalse("Did not remove item from database", found);
}
@Test
//compare order form with expected format
//order for 2 small lamps
public void testOrderFormFilledOrderLS2(){
    Printer tester = new Printer();
    DateTimeFormatter format = DateTimeFormatter.ofPattern("yyyy_MM_dd_HH_mm_ss");
    LocalDateTime time = LocalDateTime.now();
    String file1 = "orderform" + format.format(time) + ".txt";
    CostCalculator bestSet = new CostCalculator(new DatabaseReader(DBURL, username, password));
    String[] set = bestSet.calculateBestSet(2,"filing","small");
    tester.printRequest("Filing", "small", 2);
    tester.printOrderedIDs(set);
    tester.printPrice(bestSet.costOfIDSet(set, "filing"));
    tester.close();
    assertTrue("File format incorrect.", fileCompare(file1, "edu/ucalgary/ensf409/ExpectedOutputLS2.txt"));
}
@Test
//return list of manufacturers with valid category
public void testReturnManufacturersOK(){
    String result = MANUFACTURERS.returnManufacturers("chair");
    String expected = "Office Furnishings, Chairs R Us, Furniture Goods, and Fine Office Supplies.";
    
    assertTrue("Incorrect manufacturers", expected.equals(result));
}
@Test
//return list of manufacturers with invalid category
public void testReturnManufacturersInvalid(){
    String result = MANUFACTURERS.returnManufacturers("bikes");
    String expected = "Invalid category name.";
    
    assertTrue("Did not recognize false category", expected.equals(result));
}
@Test 
//check number of items for a certain type and category
public void testCheckForItem(){
    DatabaseReader tester = new DatabaseReader(DBURL, username, password);
    int count = tester.checkForItem("chair", "mesh");
    assertTrue(count == 4);
}

//adds item to Filing table in database
public void addFilingInToDatabase(String[] data){
    try{
        Connection connection = DriverManager.getConnection(DBURL, username, password);
        String query = "INSERT INTO "+data[7]+" (ID, Type, Rails, Drawers, Cabinet, Price, ManuID) VALUES (?,?,?,?,?,?,?)";
        PreparedStatement myStmt = connection.prepareStatement(query);
            
        myStmt.setString(1, data[0]);
        myStmt.setString(2, data[1]);
        myStmt.setString(3, data[2]);
        myStmt.setString(4, data[3]);
        myStmt.setString(5, data[4]);
        myStmt.setInt(6, Integer.parseInt(data[5]));
        myStmt.setString(7,data[6]);
            
        myStmt.executeUpdate();
        myStmt.close();
        connection.close();
    } catch (SQLException ex) {
        ex.printStackTrace();
    }
}
//checks for item with given ID in the database
public boolean checkForItem(String ID, String category){
    boolean isThere = false;
    try{
        Connection connection = DriverManager.getConnection(DBURL, username, password);
        Statement myStmt = connection.createStatement();
        ResultSet results = myStmt.executeQuery("SELECT * FROM "+category);

            while(results.next()){
                if(ID.equals(results.getString("ID"))){
                    isThere = true;
                    break;
                }
            }
            myStmt.close();
        connection.close();
    } catch (SQLException ex) {
        ex.printStackTrace();
    }
    return isThere;
}
    //compares 2 files with the file names as arguments
//returns true if files match and returns false otherwise
public boolean fileCompare(String file1, String file2){
    boolean areEqual = true;
    BufferedReader reader1 =null, reader2=null;
    try{
        reader1 = new BufferedReader(new FileReader(file1));
        reader2 = new BufferedReader(new FileReader(file2));
    }catch (IOException e) {
        System.err.println("Error opening files");
        System.exit(1);
    }
    try{
    String line1 = reader1.readLine();
    String line2 = reader2.readLine();
    while (line1 != null || line2 != null){
        if(line1 == null || line2 == null){
                areEqual = false;
                break;
        }
        else if(! line1.equalsIgnoreCase(line2)){
                areEqual = false;
                break;
        }

        line1 = reader1.readLine();
        line2 = reader2.readLine();
    }
}catch (IOException e) {
    System.err.println("Error reading files");
    System.exit(1);
}
try{
    reader1.close();
    reader2.close();
    }catch (IOException e) {
        System.err.println("Error closing files");
        System.exit(1);
    }
    return areEqual;
}
}

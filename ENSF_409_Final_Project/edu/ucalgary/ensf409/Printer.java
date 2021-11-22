package edu.ucalgary.ensf409;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 1.1
 */

public class Printer {
    private BufferedWriter output;

    /**
     * Initializes the file connection and also prints the general order form
     * information at the top of the file
     */
    public Printer() {
        try {
            DateTimeFormatter format = DateTimeFormatter.ofPattern("yyyy_MM_dd_HH_mm_ss");
            LocalDateTime time = LocalDateTime.now();
            output = new BufferedWriter(new FileWriter("orderform" + format.format(time) + ".txt"));
            output.write("Furniture Order Form\n\n");
            output.write("Faculty Name:\n");
            output.write("Contact:\n");
            output.write("Date:\n\n");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Prints the requested item type and number
     * @param category the category of the item
     * @param type the type of the item
     * @param num the number of items ordered
     */
    public void printRequest(String category, String type, int num) {
        try {
            output.write("Request: " + type + " " + category + ", " + num + "\n\n");
        } catch (Exception e) {
            e.printStackTrace();
        }
        
    }

    /**
     * Closes the file connection and writes to the file.
     */
    public void close() {
        try {
            output.close(); 
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Prints the item IDs that were purchased to fulfill the order
     * @param bestSet the string array of item IDs
     */
    public void printOrderedIDs(String[] bestSet) {
        try {
            output.write("Items Ordered\n");
            for (int i = 0; i < bestSet.length; i++) {
                output.write("ID: " + bestSet[i] + "\n");
            }
            output.write("\n");
        } catch (Exception e) {
            e.printStackTrace();
        }
        
    }
    
    /**
     * Outputs the price given to the method in the output file
     * @param price the price to be printed
     */
    public void printPrice(double price) {
        String temp = String.format("%.2f", price);
        try {
            output.write("Total price: $" + temp + "\n\n");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    
}

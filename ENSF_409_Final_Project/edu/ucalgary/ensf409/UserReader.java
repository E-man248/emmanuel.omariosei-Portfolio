package edu.ucalgary.ensf409;
import java.util.InputMismatchException;
import java.util.Scanner;

/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 2.7
 */

public class UserReader {

    private DatabaseReader databaseReader;
    private CostCalculator costCalculator;
    private Printer printer = null;

    private String chosenCategory;
    
    public boolean isQuit() {
        return this.quit;
    }

    private String chosenType;
    private int chosenNumberOfItems;

    private String[] storedOrder;
    private double storedOrderCost;
    private boolean quit;

    /**
     * Default constructor of the UserReader class
     */
    public UserReader(String DBURL, String USERNAME, String PASSWORD)
    {
        databaseReader = new DatabaseReader(DBURL, USERNAME, PASSWORD);     
        
        costCalculator = new CostCalculator(databaseReader);
        
        quit = false;
    }

    public static void main(String[] args)
    {
        UserReader reader = null;
        //reader = new UserReader("jdbc:mysql://localhost/inventory", "emma", "ensf409");

        try
        {
            reader = new UserReader(args[0], args[1], args[2]);
        }
        catch (Exception e)
        {
            System.out.println();
            System.out.println("Incorrect Database Input!!!");
            reader.closeProgram();
        }

        DatabaseReader databaseReader = reader.databaseReader;
        CostCalculator costCalculator = reader.costCalculator;

        // Display Introduction and Prompt the User:
        reader.displayIntroduction();

        while (!reader.quit)
        {
            // User Input Prompt:
            reader.promptUser();

            // Calculate Best Combination of Items:
            String[] bestSet = null;
            double bestPrice = Double.MAX_VALUE;

            if (databaseReader.requestedNumberIsPossible(reader.chosenNumberOfItems, reader.chosenCategory, reader.chosenType))
            {
                bestSet = costCalculator.calculateBestSet(reader.chosenNumberOfItems, reader.chosenCategory, reader.chosenType);
                bestPrice = costCalculator.costOfIDSet(bestSet, reader.chosenCategory);
            }
            else
            {
                reader.suggestManufacturers();

                System.out.println("Starting new order...");
                System.out.println();
                continue;
            }

            if (bestSet == null)
            {
                reader.suggestManufacturers();

                System.out.println("Starting new order...");
                System.out.println();
                continue;
            }         

            reader.storedOrder = bestSet;
            reader.storedOrderCost = bestPrice;

            // Purchase Confirmation:
            if (!reader.promptConfirmPurchase())
            {
                System.out.println("Cancelling your order now...");
            }
            else
            {
                System.out.println("Printing your order now...");
                if (reader.printer == null) {
                    reader.printer = new Printer();
                }

                reader.printer.printRequest(reader.chosenCategory, reader.chosenType, reader.chosenNumberOfItems);
                reader.printer.printOrderedIDs(bestSet);
                reader.printer.printPrice(bestPrice);
                
                for (int i = 0; i < bestSet.length; i++)
                {
                    databaseReader.removeItem(bestSet[i], reader.chosenCategory);
                } 
                
            }

            System.out.println();
            
            // Next Order Prompt:
            if (!reader.promptNewOrder())
            {
                reader.printer.close();
                reader.closeProgram();
            }

            System.out.println();
            System.out.println("Starting new order...");
            System.out.println();
        }
        
        reader.printer.close();
    }

    /**
     * Displays an introductory message which is printed to the console.
     * This is preferably used when the user starts the program 
     */
    public void displayIntroduction()
    {
        System.out.println("Welcome to the U of C office furniture ordering system!\n");
        System.out.println("Type QUIT (case sensitive) during any input sequence to close the program");

        System.out.println();
        
        System.out.println("Please type in the appropriate response after each"
                            + "\n" + "of the following prompts pertaining to"
                            + "\n" + "the specifications of your order.\n");
    }

    /**
     * Displays the available manufactures for a given furniture category.
     * This is preferably used when the order specified by the user is not
     * able to be completed due to lack of inventory.
     */
    public void suggestManufacturers()
    {
        System.out.print("Order cannot be fulfilled based on current inventory. Suggested manufacturers are ");
        System.out.println(MANUFACTURERS.returnManufacturers(chosenCategory));
    }

    /**
     * Prompts the user on the terminal about forming a new order and returns
     * the user's decision as a boolean.
     * This is preferably used when a previous order has just been completed
     * @return Returns a boolean representing the user's response to the prompt.
     * This is true if the user wants to form a new order and false if not.
     */
    public boolean promptNewOrder()
    {
        System.out.println("Would you like to make another order? (Y/N)");
        Scanner scanner = new Scanner(System.in);
        String verifyInput = scanner.nextLine();
        quitCheck(verifyInput);

        // If selection is not verified with a Y, the user is prompted over again
        if (!verifyInput.equalsIgnoreCase("Y"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /**
     * This class function closes the program and the input stream.
     * This is preferably used when the program is to close due to
     * an error or simply in the event that the user is finished using
     * the program.
     */
    public void closeProgram()
    {
        System.out.println();
        System.out.println("Closing program...");
        System.out.println();
        System.out.println("Have a nice day!");

        Scanner scanner = new Scanner(System.in);
        if (printer != null)
        {
            printer.close();
        }
        scanner.close();
        System.exit(0);
    }

    /**
     * Helper function. This function checks if an input String 'line' matches
     * the command for closing the program "QUIT" (must match cases).
     * If the String matches, the program is closed immediately.
     */
    private void quitCheck(String line)
    {
        if (line.equals("QUIT"))
        {
            quit = true;
            closeProgram();
        }
    }

    /**
     * Prompts the user on the terminal a series of times for their input.
     * The specific input expected from the user is indicated in the pre-prompt
     * message that appears on the terminal before an input can typed.
     * The input is stored in chosenCategory, chosenType, and chosenNumberOfItems.
     * If the input is invalid, the user will be prompted for the same input
     * again and again until an appropriate input is typed.
     */
    public void promptUser()
    {
        Scanner scanner = new Scanner(System.in);
        String inputString = null;
        boolean promptSatisfied = false;

        // Display available categories:
        System.out.println("Category Selection:");
        displayAllCategories();
        System.out.println();


        while (!promptSatisfied)
        {
            // Prompts User:
            System.out.println("Please enter the appropriate desired furniture CATEGORY below:");
            inputString = scanner.nextLine();
            quitCheck(inputString);

            // Check if input is appropriate:
            promptSatisfied = validateCategory(inputString);

            /* If input is appropriate, verify with a second prompt
             * to check if the user selection was correct */
            if (promptSatisfied)
            {
                System.out.println("Is '" + inputString + "' your desired furniture category? (Y/N)");
                String verifyInput = scanner.nextLine();
                quitCheck(verifyInput);

                // If selection is not verified with a Y, the user is prompted over again
                if (!verifyInput.equalsIgnoreCase("Y"))
                {
                    promptSatisfied = false;
                }
            }
        }
        
        // Record entry and reset:
        chosenCategory = inputString;

        inputString = null;
        promptSatisfied = false;
        scanner = new Scanner(System.in);
        System.out.println();

        // Display available types in selected category:
        System.out.println(chosenCategory.toUpperCase().charAt(0) + chosenCategory.toLowerCase().substring(1) + " Type Selection:");
        displayTypesOfCategory(chosenCategory);
        System.out.println();

        while (!promptSatisfied)
        {
            // Prompts User:
            System.out.println("Please enter the appropriate desired furniture TYPE from this category below:");
            inputString = scanner.nextLine();
            quitCheck(inputString);

            // Check if input is appropriate:
            promptSatisfied = validateType(inputString, chosenCategory);

            /* If input is appropriate, verify with a second prompt
             * to check if the user selection was correct */
            if (promptSatisfied)
            {
                System.out.println("Is '"+ inputString + "' your desired furniture type? (Y/N)");
                String verifyInput = scanner.nextLine();
                quitCheck(verifyInput);

                // If selection is not verified with a Y, the user is prompted over again
                if (!verifyInput.equalsIgnoreCase("Y"))
                {
                    promptSatisfied = false;
                }
            }
        }

        // Record entry and reset:
        chosenType = inputString;

        inputString = null;
        promptSatisfied = false;
        scanner = new Scanner(System.in);
        System.out.println();

        int inputInt = -1;


        while (!promptSatisfied)
        {
            // Prompts User:
            System.out.println("Please enter the appropriate desired NUMBER furniture of this type and category below:");
            
            // If the input is not an integer, the user is prompted again
            try
            {
                inputInt = scanner.nextInt();
            }
            catch (InputMismatchException ex)
            {
                inputString = scanner.nextLine();
                quitCheck(inputString);
                scanner = new Scanner(System.in);
                continue;
            }

            // Check if input is appropriate:
            if (inputInt > 0)
            {
                promptSatisfied = true;
            }

            /* If input is appropriate, verify with a second prompt
             * to check if the user selection was correct */
            if (promptSatisfied)
            {
                System.out.println("Is '" + inputInt + "' your desired number of furniture? (Y/N)");
                scanner = new Scanner(System.in);
                String verifyInput = scanner.nextLine();
                quitCheck(verifyInput);

                if (!verifyInput.equalsIgnoreCase("Y"))
                {
                    promptSatisfied = false;
                }
            }
        }

        // Record entry and close:
        chosenNumberOfItems = inputInt;
    }

    /**
     * Prompts the user on the terminal about whether the order they have
     * specified is correct along side the price before the item is pulled
     * and purchased.
     * @return Returns true if the user confirms the purchase and false
     * otherwise.
    */
    private boolean promptConfirmPurchase()
    {
        // Display Order:
        System.out.println();
        System.out.println("Items for Purchase:");
        for (int i = 0; i < storedOrder.length; i++)
        {
            System.out.print(storedOrder[i] + " ");
        }
        System.out.println();

        Scanner scanner = new Scanner(System.in);

        // Prompts User:

        System.out.println("Price: " + storedOrderCost);
        System.out.println();

        System.out.println("Is this the order you wish to purchase? (Y/N)");

        String verifyInput = scanner.nextLine();
        quitCheck(verifyInput);

        if (verifyInput.equalsIgnoreCase("Y"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
     * Checks to see if an input string is valid amoung a list of possible choices
     * (Note: Upper and Lower Cases are ignored!)
     * @return Returns true if the input is valid amoung the possible choices
     * and returns false otherwise.
     */
    public boolean validateCategory(String inputString)
    {
        for (int i = 0; i < INVENTORYTOARRAYS.ALLCATEGORIES.length; i++)
        {
            if (INVENTORYTOARRAYS.ALLCATEGORIES[i].equalsIgnoreCase(inputString))
            {
                return true;
            }
        }
        return false;
    }
    
    /**
     * Checks to see if an input string is valid amoung a list of possible choices
     * given the category of which the furniture belongs to. (Expects a valid category)
     * (Note: Upper and Lower Cases are ignored!)
     * @return Returns true if the input is valid amoung the possible choices
     * and returns false otherwise.
     */
    public boolean validateType(String inputString, String category)
    {
        // Category = Chair
        if (category.equalsIgnoreCase("chair"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLCHAIRTYPES.length; i++)
            {
                if (INVENTORYTOARRAYS.ALLCHAIRTYPES[i].equalsIgnoreCase(inputString))
                {
                    return true;
                }
            }
        }

        // Category = Desk
        else if (category.equalsIgnoreCase("desk"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLDESKTYPES.length; i++)
            {
                if (INVENTORYTOARRAYS.ALLDESKTYPES[i].equalsIgnoreCase(inputString))
                {
                    return true;
                }
            }
        }

        // Category = Lamp
        else if (category.equalsIgnoreCase("lamp"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLLAMPTYPES.length; i++)
            {
                if (INVENTORYTOARRAYS.ALLLAMPTYPES[i].equalsIgnoreCase(inputString))
                {
                    return true;
                }
            }
        }

        // Category = Filing Cabinet
        else if (category.equalsIgnoreCase("filing"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLFILINGTYPES.length; i++)
            {
                if (INVENTORYTOARRAYS.ALLFILINGTYPES[i].equalsIgnoreCase(inputString))
                {
                    return true;
                }
            }
        }

        else
        {
            return false;
        }

        return false;
    }

    /**
     * Prints all the categories in the InventoryToArrays inventory to the console
     */
    private void displayAllCategories()
    {
        for (int i = 0; i < INVENTORYTOARRAYS.ALLCATEGORIES.length; i++)
        {
            System.out.println(INVENTORYTOARRAYS.ALLCATEGORIES[i]);
        }
    }

    /**
     * Helper Function. Prints all the types in a specific category from InventoryToArrays inventory to the console
     * @param category The specified category
     */
    private void displayTypesOfCategory(String category)
    {
        // Category = Chair
        if (category.equalsIgnoreCase("Chair"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLCHAIRTYPES.length; i++)
            {
                System.out.println(INVENTORYTOARRAYS.ALLCHAIRTYPES[i]);
            }
        }

        // Category = Desk
        else if (category.equalsIgnoreCase("Desk"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLDESKTYPES.length; i++)
            {
                System.out.println(INVENTORYTOARRAYS.ALLDESKTYPES[i]);
            }
        }

        // Category = Lamp
        else if (category.equalsIgnoreCase("Lamp"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLLAMPTYPES.length; i++)
            {
                System.out.println(INVENTORYTOARRAYS.ALLLAMPTYPES[i]);
            }
        }

        // Category = Filing Cabinet
        else if (category.equalsIgnoreCase("Filing"))
        {
            for (int i = 0; i < INVENTORYTOARRAYS.ALLFILINGTYPES.length; i++)
            {
                System.out.println(INVENTORYTOARRAYS.ALLFILINGTYPES[i]);
            }
        }
    }

    /**
     * @return Returns the CostCalculator of the instance
     */
    public CostCalculator getCostCalculator() {
        return costCalculator;
    }

     /**
     * @return Returns the DatabaseReader of the instance
     */
    public DatabaseReader getDatabaseReader()
    {
        return databaseReader;
    }

    /**
     * @return Returns the user-chosen category of furniture
     */
    public String getChosenCategory()
    {
        return chosenCategory;
    }

    /**
     * @return Returns the user-chosen type of furniture
     */
    public String getChosenType()
    {
        return chosenType;
    }

    /**
     * @return Returns the user-chosen number of items
     */
    public int getChosenNumberOfItems()
    {
        return chosenNumberOfItems;
    }
    
    /**
     * @return Returns the quit data member
     */
    public boolean getQuit() {
        return this.quit;
    }

    /**
     * Sets the quit data member
     */
    public void setQuit(boolean quit) {
        this.quit = quit;
    }

    /**
     * Sets the chosenCategory data member
     */
    public void setChosenCategory(String chosenCategory) {
        this.chosenCategory = chosenCategory;
    }

    /**
     * Sets the chosenType data member
     */
    public void setChosenType(String chosenType) {
        this.chosenType = chosenType;
    }
    
    /**
     * Sets the ChosenNumberOfItems data member
     */
    public void setChosenNumberOfItems(int chosenNumberOfItems) {
        this.chosenNumberOfItems = chosenNumberOfItems;
    }

    /**
     * @return Returns the StoredOrder data member
     */
    public String[] getStoredOrder() {
        return this.storedOrder;
    }

    /**
     * Sets the StoredOrder data member
     */
    public void setStoredOrder(String[] storedOrder) {
        this.storedOrder = storedOrder;
    }

    /**
     * @return Returns the StoredOrderCost data member
     */
    public double getStoredOrderCost() {
        return this.storedOrderCost;
    }

    /**
     * Sets the StoredOrderCost data member
     */
    public void setStoredOrderCost(double storedOrderCost) {
        this.storedOrderCost = storedOrderCost;
    }

}

package edu.ucalgary.ensf409;

/**
 * @author Emmanuel Omari-Osei | UCID: 30092729
 * @author Nicole Zacaruk | UCID: 30100135
 * @author Evyn Rissling | UCID: 30096936
 * @author Priyanka Gautam | UCID: 30091244
 * @version 1.0
 */

public final class MANUFACTURERS {
    public static final int[] FILINGMANUFACTURERSID = {2, 4, 5};

    public static final int[] CHAIRMANUFACTURERSID = {2, 3, 4, 5};

    public static final int[] DESKMANUFACTURERSID = {1, 2, 4, 5};

    public static final int[] LAMPMANUFACTURERSID = {2, 4, 5};

    public static final String[] MANUFACTURERS = 
    {
        "Academic Desks",
        "Office Furnishings",
        "Chairs R Us",
        "Furniture Goods",
        "Fine Office Supplies"
    };

    public static String returnManufacturers(String category) {
        int[] array; 
        String temp = "";

        if (category.equalsIgnoreCase("filing")) {
            array = FILINGMANUFACTURERSID;
        } else if (category.equalsIgnoreCase("chair")) {
            array = CHAIRMANUFACTURERSID;
        } else if (category.equalsIgnoreCase("desk")) {
            array = DESKMANUFACTURERSID;
        } else if (category.equalsIgnoreCase("lamp")) {
            array = DESKMANUFACTURERSID;
        } else {
            return "Invalid category name."; // for debug
        }

        boolean firstTime = true;
        for (int i = 0; i < array.length; i++) {
            int index = array[i];
            if (firstTime) {
                temp += MANUFACTURERS[index - 1];
                firstTime = false;
            } else if (i == array.length - 1) {
                temp += ", and " + MANUFACTURERS[index - 1] + '.';
            } else {
                temp += ", " + MANUFACTURERS[index - 1];
            }
        }

        return temp;
    }
}

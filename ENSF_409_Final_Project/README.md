# ENSF_409_Final_Project

Welcome to our Inventory Application!

-------------------------------------------------------------------------------------------------------------------
To Run the Application:

In order to properly run and install the file, download the zip file from the Dropbox. Once the file is downloaded and unzipped, navigate from the command-line prompt (or terminal window) to the folder for the project: 
ENSF_409_Final_Project.

In order to compile all of the java files that are located in the package edu/ucalgary/ensf409, after writing "javac", write the path to the MySQL Connector Jar file which is included in the lib folder.

The relative path to the MySQL Connector Jar file is as follows:

lib\mysql-connector-java-8.0.23.jar

For example your COMPILATION COMMAND will look like this:
(Note: Windows Command Prompt Example! Might look different in another terminal window!)

javac -cp .;lib/mysql-connector-java-8.0.23.jar edu/ucalgary/ensf409/UserReader.java

Now in order to run the file, you must write "java" on the command-line prompt and then write the path to your My SQL Connector Jar file, then the path to UserReader class. UserReader is where the static main is located. After writing the path to UserReader, you must then write the path to the inventory JDBC which is:

jdbc:mysql://localhost/inventory

After the final args are the Username and password of the MySQL user of your choice. For example, If I picked the user, "emily" and her password was, "ensf409", then my complete command line would look like the following:

java -cp .;lib/mysql-connector-java-8.0.23.jar edu/ucalgary/ensf409/UserReader jdbc:mysql://localhost/inventory emily ensf409

To Run the Unit Tests:

All the unit tests are in one file, UserReaderTest.java, in the edu/ucalgary/ensf409 package. The necessary .jar files needed to run the JUNIT program are contaoned in the lib folder.

An example of the command line needed to COMPILE the JUNIT testing file in a windows system is as follows:

javac -cp .;lib/hamcrest-core-1.3.jar;lib/system-rules-1.19.0.jar;lib/junit-4.13.2.jar;lib/mysql-connector-java-8.0.23.jar;. edu/ucalgary/ensf409/UserReaderTest.java

An example of the command line needed to RUN the JUNIT testing file in a windows system is as follows:

java -cp .;lib/junit-4.13.2.jar;lib/hamcrest-core-1.3.jar;lib/system-rules-1.19.0.jar;lib/mysql-connector-java-8.0.23.jar;. org.junit.runner.JUnitCore edu.ucalgary.ensf409.UserReaderTest

-------------------------------------------------------------------------------------------------------------------
A few notes about the application:
- The static main is located in class UserReader 
- The output form that is produced has a title of "orderform" and has a timestamp in the title of the form: yy/mm/dd/hour/minute/second
An example of an order form that was produced is: "orderform2021_04_12_22_39_30.txt"
- When testing JUNIT, the default user that is used by the program is "scm" that has the password "ensf409". However if you would like to use a different user, then you can manually change the username and password of the user in class variables at the top of the UserReaderTest.java file, private final string username and private final string password 
- The unit tests have hardcoded values for expected outputs and therefor requires the original inventory to be successful
- The unit tests do not change the database and the inventory should not need to be refreshed after the program has run







￼

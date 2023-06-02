
# Quiz - Spis treści
1. [Introduction](#introduction)
2. [How to run the program](#how-to-run-the-program)
3. [Application overview](#application-overview)

# Introduction
Quiz is ASP.NET Core application created to learn the framework. It is built in client-server architecture. It allows you to create your own test questions and solve tests. The technologies I used are ASP.NET Core, Blazor & SQL.

# How to run the program
1. You should have .NET 7.0 installed.
2. Create database in MS SQL Server.
3. Open the `database/schema.sql` file in MS SQL Managament Studio and execute it in the context of the database that you created in step 2.
4. Open `Quiz/Quiz.WebApi/appsettings.json` and replace the connection string with connection string to your databse.
5. (optional) Now you can add a sample data of questions, which I encourage you to do. This will make it easier for you to check the functionality of the program. Execute the `database/sample-data-seed.sql` script.

6. Expand start and select "Configure Startup Projects"

![image](https://github.com/JustynaPowala/Quiz/assets/124584877/ada96428-ed2c-4ace-8ae6-a93a2472ebbd)

Set "Start" for Quiz.WebApi & Quiz.WebUI
![image](https://github.com/JustynaPowala/Quiz/assets/124584877/874b2951-ba4a-4833-8a62-5a853cc2937b)

Then start the program.

# Application overview

The UI has two aspects: Questions Designer and Test.

**Questions Designer**
![image](https://github.com/JustynaPowala/Quiz/assets/124584877/2aaa771f-8366-4f24-8286-cf7bb7682b1b)

In this section, you can create new questions from the given categories, choose the number of points for a given question and the multiplicity of selections. After pressing "Save", the question will be added to the database with the InPreparation status and you will be able to add an answer to the question. When you consider that the question is ready, press the "Confirm" button, then the question will become Active. Only questions with the Active status are assigned to the tests.

On the right is a list of questions. You can filter them by content and/or category. You can modify InPreparation questions by clicking the tick in the "Select" column. Active questions cannot be modified, but you have the option to clone such a question. Then the old version will get the Archived status.

If you want to add a new category, enter the Quiz.WebAPI project in HardcodedCategoriesProvider and add it to the categories that already exist there.
![image](https://github.com/JustynaPowala/Quiz/assets/124584877/2e4a42de-c617-49d0-9786-f8e936b099f9)

**Test**
In this section, first select the categories from which you want to take the test.

![image](https://github.com/JustynaPowala/Quiz/assets/124584877/75e3414f-ca52-407e-b3d1-7487e16a0cf7)

Press "Start" when you're ready.

![image](https://github.com/JustynaPowala/Quiz/assets/124584877/03defed2-2d9b-4753-9b87-318fe24016f5)


Then start taking the test. "Next" displays the next question, "Back" the previous one. When you think you are done, press Finish, then the result will be displayed.

![image](https://github.com/JustynaPowala/Quiz/assets/124584877/dff7a53d-392e-4ee1-9f73-87c519dc3480)






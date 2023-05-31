#[In preparation]
# Quiz - Spis treści
1. [Introduction](#introduction)
2. [How to run the program](#how-to-run-the-program)
3. [Application overview](#application-overview)

# Introduction
Quiz is a web application created by me during my participation in the TechLeaders mentoring program. It is built in client-server architecture. It allows you to create your own test questions and solve tests. The technologies I used areare .NET, Blazor & SQL.

# How to run the program
1. .Net
2. schema.sql 
3. appsetting
4. optional sample data seed.sql
5. Open both 
6. ...

# Application overview

The UI has two aspects: Questions Designer and Test.
Questions Designer
![image](https://github.com/JustynaPowala/Quiz/assets/124584877/2aaa771f-8366-4f24-8286-cf7bb7682b1b)

In this section, you can create new questions from the given categories, choose the number of points for a given question and the multiplicity of selections. After pressing "Save", the question will be added to the database with the InPreparation status and you will be able to add an answer to the question. When you consider that the question is ready, press the "Confirm" button, then the question will become Active. Only questions with the Active status are assigned to the tests.

On the right is a list of questions. You can filter them by content and/or category. You can modify InPreparation questions by clicking the tick in the "Select" column. Active questions cannot be modified, but you have the option to clone such a question. Then the old version will get the Archived status.

If you want to add a new category, enter the Quiz.WebAPI project in HardcodedCategoriesProvider and add it to the categories that already exist there.
![image](https://github.com/JustynaPowala/Quiz/assets/124584877/2e4a42de-c617-49d0-9786-f8e936b099f9)



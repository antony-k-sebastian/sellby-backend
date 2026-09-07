Feature: Login
    As a student 
    I want to log into Sellby 
    So that I can access the marketplace

    Scenario: Successfull login with valid credentials
        Given the user is on login page
        When they enter a valid email and password
        And they click the login button
        Then they should be redirected to the marketplace homepage.

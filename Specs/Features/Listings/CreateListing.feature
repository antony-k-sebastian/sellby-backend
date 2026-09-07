Feature: Create listing
    As a seller
    I want to create a listing
    So that buyers can find items I'm selling

Scenario: Successfully creating a listing
    Given I am authenticated as a seller
    And the category "Electronics" exists
    When I create a listing with title "Vintage Camera", price 120.00 and category "Electronics"
    Then the response status should be 201
    And the created listing should have title "Vintage Camera"

Scenario: Rejecting a listing with a blank title
    Given I am authenticated as a seller
    And the category "Electronics" exists
    When I create a listing with title "" , price 120.00 and category "Electronics"
    Then the response status should be 400
    And the response should contain the message "Title is required."

Scenario: Rejecting a listing with a negative price
    Given I am authenticated as a seller
    And the category "Electronics" exists
    When I create a listing with title "Vintage Camera", price -10.00 and category "Electronics"
    Then the response status should be 400
    And the response should contain the message "Price cannot be negative."

Scenario: Rejecting a listing with an unknown category
    Given I am authenticated as a seller
    When I create a listing with title "Vintage Camera", price 120.00 and an unknown category
    Then the response status should be 400
    And the response should contain the message "Category not found."

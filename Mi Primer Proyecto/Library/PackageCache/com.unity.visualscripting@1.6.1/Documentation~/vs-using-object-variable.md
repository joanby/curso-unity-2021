# Using an object variable

Object variables are variables that are contained in the Variables component and can be used in your script graph. For example, exposing health on a game object.

In order to see the variables, select the game object and look at the variables component in the Inspector or in the Blackboard in the Object tab.

Note: In the Blackboard, the Object variable tab is enabled when you select a game object with a Variable component on it, after which you only see the variables from the selected game object in this tab. Selecting the script graph asset from the Project window grays out the Object tab. 

### To use object variables

1. Add a script machine.  
   Unity automatically adds a variable component to the game object.  These are for Object 
   variables.  
   Note: Object variables are shared across the entire object. Multiple graphs attached to the object can all use the same object variables. Do not use an object variable that does not exist on an object because a user needs to add all the object variables by hand.
2. Select **Edit Graph**.
3. Add units to the graph.
4. In the Variable component, enter the name of the new variable you want to use.
5. Click the (**+**) button to add this new variable.  
   The newly created variable appears in the variables list and the value is set to null.
6. Click on the drop-down menu to change the name and type of the variable.
7. Drag and drop the new variable to the graph by dragging on the handles. 
   

You can now use this variable inside the graph.

Warning: Renaming the variable inside the Variable component won’t be reflected in the graph.


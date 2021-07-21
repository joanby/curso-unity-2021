

# Creating a custom event unit



A custom event unit can trigger your own events across graphs, along with custom arguments. To trigger the custom event from elsewhere, you need to use a Trigger Custom Event unit.

### To create a custom event

In a script graph:

1. Right-click on an empty spot.</br>
   The fuzzy finder appears.
2. Select **Events** > **Custom Event**.</br>
   Unity places a Custom Event unit in the graph.</br>
   Note: The first argument is indexed Arg 0. The arguments are the values the event can process. If you want extra data ports, increase the Arguments field to the number of data ports (for example four data ports means Arguments = 4).



Note: The sender and receiver units must have the same number of arguments. 



### To trigger a custom event

In a script graph:

1. Right-click on an empty spot.</br>
   The fuzzy finder opens.

2. Select **Events** > **Custom Event** > **Trigger Custom Event**.</br>
   Note: The name of the event must be the same as the Custom Event (the label is case- and space-sensitive).</br>
   The GameObject containing the script machine with the event you want to trigger must be connected to the GameObject’s data port.</br>
   Unity places a Trigger Custom Event unit in the graph.</br>
   The first argument is indexed Arg 0. The arguments are the values that the event can process. If you want extra data ports, increase the Arguments field to the number of data ports (for example four data ports means Arguments = 4).



Note: The sender and receiver units must have the same number of arguments. Alsol, when you trigger the event, there need to be values associated with the arguments, even if the values are not used. All arguments through data ports need to be connected to another unit or you receive error messages.



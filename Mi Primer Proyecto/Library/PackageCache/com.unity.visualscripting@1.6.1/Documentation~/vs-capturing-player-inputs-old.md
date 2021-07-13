

# Capturing player inputs using the input manager

To use units from the input system (input manager), the Input unit must be linked from its output data port or its input trigger to a unit. To use the old input system, set the **Edit** > **Project Settings** > **Player** > **Active Input Handling** to Input Manager (Old) or Both.

Note: The Input Manager (**Edit** > **Project Settings** > **Input Manager**) lists all the input types.

### To enter an input in the system

1. In a script graph that has an Event unit (for example Update Event), right-click on an empty spot on the graph.</br>
   A command list appears.
2. Select **Add Unit**.
   The fuzzy finder appears.
3. In the search field, enter “get axis”.
4. Select **Input: Get Axis**.</br>
   The Get Axis unit appears on the graph.
5. Label the unit in the **axisName** field (for example Horizontal).</br>
   WARNING: You must label the unit with the exact spelling of the units listed in the Input Manager or Unity does not recognize them.</br>
   TIP: Copy and paste the input unit name to ensure the spelling is correct.
6. Drag the output port from an event unit to the input port of the Input unit. Release the arrow (mouse button) so the two units are connected. </br>
   Note: Every time there is a frame cycle and if the data port is used, the Input unit receives a signal.
7. From the **Get Axis** unit, drag from the output trigger port to an input port in another unit (for example a Transform unit).</br>
   Every time the user clicks the key for the Get Axis unit (for example, the left or right arrow), the downstream unit increments.

Creating an input unit using this method might not guarantee you’ve selected a unit that is compatible with the selected event. Use the fuzzy finder by dragging from the event output port:only compatible units (that is, units that can be linked from that event) appear in the fuzzy finder. 



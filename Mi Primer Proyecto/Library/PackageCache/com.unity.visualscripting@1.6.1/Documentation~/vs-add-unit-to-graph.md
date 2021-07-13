# Adding a unit to a graph

The script graph implements game play logic. There are several ways to add a unit to a flow graph. The two most common methods of adding a unit to a flow graph are detailed in the following process.

Note: When you create a new script graph, a Start event unit and an Update event unit are automatically added to the script graph. The Start unit runs only once, when the script graph is initialized. The Update unit runs continuously as long as the GameObject is active.

### To add a unit to a graph

1. Open a Flow graph.
2. Do one of the following:
   - Right-click an empty space on the graph.</br>
     The fuzzy finder appears with all categories of units.</br>
     Select a unit by scrolling through the fuzzy finder or by entering its name in the search field.</br>
     The unit is placed in the graph.</br>
     Connect the unit to another unit through the input/output ports.
   - Drag and release from any trigger or data port (that is, the trigger port with the arrow icon, or the data port, which has any other icon) on a unit.</br>
     A list of all units that are compatible with the source unit appears.</br>
     Select one (by scrolling or entering its name in the fuzzy field).
     The selected unit is placed in the graph connected to the first unit.



/************************************

Run Super-RPC-POC server, which is containing the process explorer collector (collecting the data from clients, and main process).

Information:

        - Under ClientBehavior folder you can find a CommunicatorHelper.cs, which is a helper class, implementing the behavior/communication if a process has been changed. Right now it will just print a message to the console. 

        ///
        (if a process id modified then the SendMessage(which recieves the messages) function will get a ProcessInfoDto if a process is terminated then it will get a PID.)
        ///

        - Under Example folder you can find the MiddlewareV2.cs file. There can be the host functions/objects etc registered. Those information can be requested from server through Super-RPC. 

        - Also it will create an another process (Notepad.exe) and collects all the modification in the server.
        
        - Right now it doesn't push the changes on the UI, but it writes logs.

        - Under wwwroot there is a sample js client (implementing the remote objects).
        `
************************************/

/************************************

If you want to test the client side, run the SuperRPC-POC-Client program too, the server will get the data from this module and can send it to the UI.

Information:

        - If we want to build an another client which communicates with Super-RPC it will need the interface of the requested remote objects -  if we want to use them (in our case IInfoCollectorServiceObject).

        - The ServiceObject.cs will collect the data what we want to send to the server. We have created a DummyConnection, what will close after some time. It can be distracting, because right now that connectionstatus is already stopped, because we have never opened it, but we will trigger a method to show that it will send the info to the server.

        ///
        (you can set breakpoints on the server to watch this communication (InfoServiceObject.cs ProcessStatusChanged(object conn) method), it will writew to the console)
        ///
************************************/

/************************************
To watch that the UI gets the processes for now, start the UI with npm start, and open the developer console. (npm install might be needed too.)
************************************/
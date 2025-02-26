
package org.example.Utils;

import org.example.IFirmaServices;
import org.example.FirmaClientRPCWorker;

import java.net.Socket;

public class RPCConcurrentServer extends AbstractConcurrentServer {

    private IFirmaServices firmaServices;
    public RPCConcurrentServer(int port, IFirmaServices firmaServices) {
        super(port);
        this.firmaServices = firmaServices;
        System.out.println("RPCConcurrentServer");
    }

    @Override
    protected Thread createWorker(Socket client) {
        FirmaClientRPCWorker worker = new FirmaClientRPCWorker(firmaServices, client);
        Thread tw = new Thread(worker);
        return tw;
    }
}

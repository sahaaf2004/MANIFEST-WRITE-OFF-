import express from "express";
import { createServer as createViteServer } from "vite";
import path from "path";
import { fileURLToPath } from "url";
import dotenv from "dotenv";

dotenv.config();

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

async function startServer() {
  const app = express();
  const PORT = 3000;

  app.use(express.json());

  // API routes
  app.post("/api/validate", async (req, res) => {
    // Mocking validation logic for now
    res.json({ status: "success" });
  });

  app.post("/api/amendment/submit", async (req, res) => {
    res.json({ amendment_ref: `AMD-${new Date().getFullYear()}-${Math.floor(Math.random() * 1000000)}`, status: "pending_review" });
  });

  app.get("/api/amendment/status/:ref", async (req, res) => {
    res.json({ status: "approved" });
  });

  app.post("/api/payment/initiate", async (req, res) => {
    res.json({ payment_ref: `PAY-${Math.floor(Math.random() * 1000000)}`, status: "initiated" });
  });

  app.post("/api/payment/confirm", async (req, res) => {
    res.json({ status: "confirmed" });
  });

  app.post("/api/writeoff/finalize", async (req, res) => {
    res.json({ writeoff_ref: `WO-${new Date().getFullYear()}-${Math.floor(Math.random() * 1000000)}`, finalized_at: new Date().toISOString(), certificate_url: "/cert.pdf" });
  });

  // Vite middleware for development
  if (process.env.NODE_ENV !== "production") {
    const vite = await createViteServer({
      server: { middlewareMode: true },
      appType: "spa",
    });
    app.use(vite.middlewares);
  } else {
    const distPath = path.join(process.cwd(), "dist");
    app.use(express.static(distPath));
    app.get("*", (req, res) => {
      res.sendFile(path.join(distPath, "index.html"));
    });
  }

  app.listen(PORT, "0.0.0.0", () => {
    console.log(`Server running on http://localhost:${PORT}`);
  });
}

startServer();

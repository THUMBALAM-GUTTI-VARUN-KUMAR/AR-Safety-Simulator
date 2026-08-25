# Backend

This directory contains the Python FastAPI backend and database management scripts for the AR Safety Simulator.

## Tech Stack
- Python
- FastAPI
- PostgreSQL (via SQLAlchemy)

## Setup & Configuration

1. **Environment Variables**:
   Copy the `.env.example` file to `.env`:
   ```bash
   cp .env.example .env
   ```
   Update the `DATABASE_URL` with your PostgreSQL credentials. 
   *(Note: If you don't configure PostgreSQL, the system will fallback to a local SQLite database for rapid prototyping/testing).*

2. **Virtual Environment**:
   It's recommended to run the app in a virtual environment.
   ```bash
   python -m venv venv
   # On Windows:
   venv\Scripts\activate
   # On macOS/Linux:
   source venv/bin/activate
   ```

3. **Install Dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

## Running the Server

Start the FastAPI development server:
```bash
uvicorn main:app --reload
```
The server will run at `http://127.0.0.1:8000`. The database tables will be created automatically upon startup.

## Testing the Endpoints

There are several ways to test the functionality:

1. **Swagger UI**: Visit [http://127.0.0.1:8000/docs](http://127.0.0.1:8000/docs) in your browser. This provides an interactive UI to test all APIs.
2. **Automated Test Script**: While the server is running, execute the provided test script in a new terminal window:
   ```bash
   python test_endpoints.py
   ```
   This will automatically test the `POST` and `GET` endpoints with dummy data according to the API Contract.

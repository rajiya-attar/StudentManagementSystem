# 📚 Student Management System - Usage Guide

## 🚀 API Running Hai!

API Address: **http://localhost:5000**

---

## 📝 Step-by-Step Usage

### Step 1: Browser mein Swagger UI open karo
```
http://localhost:5000/swagger
```

### Step 2: Pehle User Register Karo
**Endpoint:** `POST /api/auth/register`

Body (JSON):
```json
{
  "email": "yourname@test.com",
  "password": "password123",
  "confirmPassword": "password123",
  "fullName": "Your Full Name"
}
```

### Step 3: Login Karo Token Paane ke liye
**Endpoint:** `POST /api/auth/login`

Body (JSON):
```json
{
  "email": "yourname@test.com",
  "password": "password123"
}
```

Response mein milega:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

### Step 4: Swagger mein Authorize Button click karo
- Upar "Authorize" button dikhega
- Uspe click karo
- Type karo: `Bearer eyJhbGciOiJIUzI1NiIs...` (jo token mila tha)
- Authorize karo

### Step 5: Students APIs use karo

#### Sab Students Dekho
```
GET /api/students
```

#### Ek Student Dekho
```
GET /api/students/1
```

#### Naya Student Add Karo
```
POST /api/students
```
Body:
```json
{
  "name": "Rahul Kumar",
  "email": "rahul@test.com",
  "age": 22,
  "course": "Computer Science"
}
```

#### Student Update Karo
```
PUT /api/students/1
```
Body:
```json
{
  "name": "Rahul Updated",
  "email": "rahul@test.com",
  "age": 23,
  "course": "Software Engineering"
}
```

#### Student Delete Karo
```
DELETE /api/students/1
```

---

## 🔐 Important
- **Register** aur **Login** public hai (no token needed)
- **Students** APIs ke liye JWT token chahiye
- Token expire ho jayega (60 minutes)

---

## 🧪 Testing with Postman (Optional)

1. Postman download karo
2. New Request banao
3. URL: `http://localhost:5000/api/auth/register`
4. Method: POST
5. Body → Raw → JSON
6. JSON data daalo
7. Send karo

---

## ❓ Common Issues

### 404 Error on Swagger
- Server restart karo
- URL check karo: `http://localhost:5000/swagger`

### 401 Unauthorized Error
- Token add karo Swagger ke Authorize button se
- Token expired toh dobara login karo

### Database Error
- `StudentManagement.db` file delete karo
- Server restart karo (auto recreate hoga)

---

## 🎯 Quick Test URLs

Sab kuch test karne ke liye:

1. Register: http://localhost:5000/api/auth/register
2. Login: http://localhost:5000/api/auth/login  
3. Get Students: http://localhost:5000/api/students
4. Swagger UI: http://localhost:5000/swagger

---

**Ready to use!** 🎉

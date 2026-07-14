import { Link } from "react-router-dom";

const NotFound = () => {
  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <div className="text-center">
        <h1 className="text-9xl font-bold text-primary-100">404</h1>
        <h2 className="text-3xl font-bold text-gray-800 mt-4">
          Page Not Found
        </h2>
        <p className="text-gray-500 mt-3 mb-8">
          The page you're looking for doesn't exist.
        </p>
        <Link
          to="/"
          className="inline-block px-8 py-3 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 transition"
        >
          Go Home
        </Link>
      </div>
    </div>
  );
};

export default NotFound;
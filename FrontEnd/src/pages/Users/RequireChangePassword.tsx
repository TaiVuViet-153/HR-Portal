import ChangePasswordModal from '@/pages/Users/ChangePasswordModal';

export default function RequireChangePassword() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-6">
      <ChangePasswordModal
        isOpen={true}
        onClose={() => {}}
        forceRequired={true}
        title="Change Password Required"
      />
    </div>
  );
}

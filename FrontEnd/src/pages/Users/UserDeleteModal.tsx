import Modal from '@/components/common/Modal';
import { Trash2 } from 'lucide-react';

type UserDeleteModalProps = {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: (e: React.FormEvent) => void;
    userName?: string;
};

export default function UserDeleteModal({ isOpen, onClose, onConfirm, userName }: UserDeleteModalProps) {
    return (
        <Modal 
            isOpen={isOpen} 
            onClose={onClose} 
            title="Delete Employee"
            size="sm"
            footer={(
                <>
                    <button 
                        onClick={onClose} 
                        className="px-6 py-2.5 text-sm font-bold text-gray-600 hover:bg-gray-100 rounded-xl transition-all cursor-pointer"
                    >
                        Go Back
                    </button>
                    <button 
                        onClick={onConfirm} 
                        className="px-6 py-2.5 text-sm font-bold bg-red-600 text-white rounded-xl hover:bg-red-700 transition-all shadow-lg shadow-red-100 cursor-pointer"
                    >
                        Delete Employee
                    </button>
                </>
            )}
        >
            <div className="text-center py-4">
                <div className="w-20 h-20 bg-red-50 text-red-600 rounded-3xl flex items-center justify-center mx-auto mb-6">
                    <Trash2 size={40} />
                </div>
                <h3 className="text-lg font-bold text-gray-900 mb-2">Delete this employee?</h3>
                <p className="text-gray-500 font-medium">
                    {userName ? (
                        <>This will permanently remove <span className="font-bold text-gray-700">{userName}</span> from the system.</>
                    ) : (
                        <>This will permanently remove the employee from the system.</>
                    )}
                </p>
            </div>
        </Modal>
    );
}

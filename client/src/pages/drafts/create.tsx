import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { type DraftFormData } from "../../models/generated-client";
import { useForm, type SubmitHandler } from "react-hook-form";
import { draftClient } from "../../api-clients";

export default function DraftCreate() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DraftFormData>();
  const navigate = useNavigate();

  const onSubmit: SubmitHandler<DraftFormData> = async (data) => {
    await toast.promise(draftClient.create(data), {
      success: "Draft created successfully",
      error: "Draft creation failed",
      loading: "Creating draft...",
    });
    navigate("..");
  };

  return (
    <div className="card w-full bg-neutral shadow-xl">
      <div className="card-body">
        <h2 className="card-title">Create post</h2>
        <form method="post" onSubmit={handleSubmit(onSubmit)}>
          <div className="form-control">
            <label className="label">
              <span className="label-text">Title</span>
            </label>
            <input
              placeholder="Title"
              type="text"
              className={`input input-bordered w-full max-w-xs ${
                errors.title && "input-error"
              }`}
              {...register("title", { required: true })}
            />
            <small className="text-error">{errors.title?.message}</small>
          </div>
          <div className="form-control">
            <label className="label">
              <span className="label-text">Content</span>
            </label>
            <textarea
              placeholder="Content"
              className={`textarea textarea-bordered w-full ${
                errors.content && "input-error"
              }`}
              {...register("content", { required: true })}
            />
            <small className="text-error">{errors.content?.message}</small>
          </div>
          <div className="form-control">
            <label className="label cursor-pointer">
              <span className="label-text">Publish</span>
              <input
                type="checkbox"
                className="toggle"
                {...register("publish", { value: false })}
              />
            </label>
          </div>
          <div className="card-actions justify-end">
            <button type="submit" className="btn btn-primary">
              Save
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
